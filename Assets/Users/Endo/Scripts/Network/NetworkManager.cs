using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public enum EventType
{
    Init,
    Match,
    Joined,
    PlayerMove,
    BulletMove,
    RoundUpdate,
    Disconnect,
    Refresh,
    Error
}

public class NetworkManager : SingletonMonoBehaviour<NetworkManager>
{
    [SerializeField, Header("サーバーアドレス")]
    private string address = "localhost";

    [SerializeField, Header("サーバーポート")]
    private int port = 6080;

    private static UdpClient _client;
    private static Thread    _thread;

    private static Dictionary<string, GameObject> _players;

    private static Subject<SendData> _receiverSubject;

    /// <summary>
    /// 自身がホストか。部屋を立てたプレイヤーならtrueとなる。
    /// </summary>
    public static bool IsOwner { get; private set; }

    public static IObservable<SendData> OnReceived => _receiverSubject;

    public static bool   IsConnected  { get; private set; } // サーバーに接続中か
    public static string RivalAddress { get; private set; }
    public static int    RivalPort    { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        _receiverSubject       = new Subject<SendData>();
        SelfPlayerData.Address = address;
        SelfPlayerData.Port    = port;

        Init();
        DontDestroyOnLoad(this);
        OnReceived.Subscribe(EventHandler);
    }

    private void OnDestroy()
    {
        Disconnect();
    }

    private void OnApplicationQuit()
    {
        Disconnect();
    }

    /// <summary>
    /// 接続情報を初期化する
    /// </summary>
    private static void Init()
    {
        _players    = new Dictionary<string, GameObject>();
        _client     = null;
        _thread     = null;
        IsConnected = false;
        IsOwner     = false;
    }

    /// <summary>
    /// サーバーに非同期で接続する
    /// </summary>
    /// <returns>正常に接続できたか</returns>
    public static async UniTask Connect()
    {
        if (IsConnected) return;

        _client ??= new UdpClient();

        // 初回接続時、応答が帰ってくるか確認するためタイムアウト設定。帰ってきたら解除
        _client.Client.ReceiveTimeout = 5000;

        try
        {
            // 接続完了まで待機
            await UniTask.Run(() => _client.Connect(Instance.address, Instance.port));

            UniTask.Run(ReceiveData);

            IsConnected = true;
        }
        catch (Exception e)
        {
            // TODO: UIで表示
            // TODO: 再試行処理
            Debug.LogError($"サーバーへの接続時にエラーが発生しました: {e.Message}");
        }
    }

    /// <summary>
    /// サーバーから切断する
    /// </summary>
    private static void Disconnect()
    {
        if (!IsConnected) return;

        var data = new SendData(EventType.Disconnect)
        {
            Self = new PlayerData()
        };

        Emit(data);
        Init();
        _client?.Close();
        _thread?.Abort();

        _client = null;
    }

    /// <summary>
    /// サーバーからのレスポンスを受信し、イベントを発行する
    /// </summary>
    private static async void ReceiveData()
    {
        try
        {
            while (true)
            {
                IPEndPoint ep       = null;
                byte[]     received = _client.Receive(ref ep);
                string     msg      = Encoding.UTF8.GetString(received);

                SendData data = SendData.MakeJsonFrom(msg);
                _receiverSubject.OnNext(data);
            }
        }
        catch (SocketException e)
        {
            // UIを書き換えるためメインスレッドに戻す（暫定）
            await UniTask.SwitchToMainThread();

            Debug.LogError(e.Message);
            SystemProperty.StatusText.text = "サーバーに接続できませんでした";
            IsConnected                    = false;
        }
    }

    /// <summary>
    /// サーバーにデータを送信する
    /// </summary>
    /// <param name="data">送信データ</param>
    public static void Emit(SendData data)
    {
        if (!IsConnected) return;

        string msg      = SendData.ParseSendData(data);
        byte[] sendData = Encoding.ASCII.GetBytes(msg);
        _client.Send(sendData, sendData.Length);
    }

    /// <summary>
    /// サーバーからのレスポンスのイベントに応じて処理を行う
    /// </summary>
    /// <param name="data">受信データ</param>
    private static void EventHandler(SendData data)
    {
        var type = (EventType) Enum.Parse(typeof(EventType), data.Type);

        switch (type)
        {
            case EventType.Init:
            {
                _client.Client.ReceiveTimeout = 0;
                SelfPlayerData.Uuid           = data.Self.Uuid;

                // TODO: マップへの参加時にオブジェクト代入
                _players.Add(data.Self.Uuid, null);

                break;
            }

            case EventType.Match:
            {
                // 相手情報を格納
                // OPTIMIZE: 後発で参加したプレイヤーのUUIDがSelfで帰ってくるため、どうにかする
                if (data.Self.Uuid == SelfPlayerData.Uuid)
                {
                    _players.Add(data.Rival.Uuid, null);
                    RivalAddress = data.Rival.Address;
                    RivalPort    = data.Rival.Port;
                }
                else
                {
                    _players.Add(data.Self.Uuid, null);
                    RivalAddress = data.Self.Address;
                    RivalPort    = data.Self.Port;
                    IsOwner      = true;
                }

                break;
            }

            case EventType.Refresh:
            {
                KeyValuePair<string, GameObject>[] players = _players.ToArray();

                foreach (KeyValuePair<string, GameObject> player in players)
                {
                    if (data.Rival.Uuid != player.Key) continue;

                    _players.Remove(player.Key);
                }

                // TODO: 対戦相手切断UI表示 && 状況に応じてタイトルに戻す

                break;
            }

            case EventType.Error:
            {
                Debug.LogError(data.Message);

                break;
            }
        }
    }
}
