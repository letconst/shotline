using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UniRx;
using UnityEngine;

public enum EventType
{
    Init,
    Join,
    Move,
    Disconnect,
    ClientReload
}

public class NetworkManager : SingletonMonoBehaviour<NetworkManager>
{
    [SerializeField, Header("サーバーアドレス")]
    private string address = "localhost";

    [SerializeField, Header("サーバーポート")]
    private int port = 6080;

    private static bool      _isConnected; // サーバーに接続中か
    private static UdpClient _client;
    private static Thread    _thread;

    private static Dictionary<string, GameObject> _players;

    private static Subject<SendData> _receiverSubject;

    /// <summary>
    /// 自身がホストか。部屋を立てたプレイヤーならtrueとなる。
    /// </summary>
    public static bool IsOwner { get; private set; }

    public static IObservable<SendData> OnReceived => _receiverSubject;

    public static string SelfAddress  => Instance.address;
    public static int    SelfPort     => Instance.port;
    public static string RivalAddress { get; private set; }
    public static int    RivalPort    { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        _receiverSubject = new Subject<SendData>();

        Init();
        DontDestroyOnLoad(this);

        Connect();
    }

    private void OnDestroy()
    {
        Disconnect();
    }

    private void OnApplicationQuit()
    {
        Disconnect();
    }

    private static void Init()
    {
        _players     = new Dictionary<string, GameObject>();
        _client      = null;
        _thread      = null;
        _isConnected = false;
        IsOwner      = false;
    }

    /// <summary>
    /// サーバーに接続する
    /// </summary>
    /// <returns>正常に接続できたか</returns>
    public static bool Connect()
    {
        if (_isConnected) return false;

        _client ??= new UdpClient();

        try
        {
            _client.Connect(Instance.address, Instance.port);

            _thread = new Thread(ReceiveData);
            _thread.Start();

            _isConnected = true;

            return true;
        }
        catch (Exception e)
        {
            // TODO: UIで表示
            // TODO: 再試行処理
            Debug.LogError($"サーバーへの接続時にエラーが発生しました: {e.Message}");

            return false;
        }
    }

    private static void ReceiveData()
    {
        while (true)
        {
            IPEndPoint ep       = null;
            byte[]     received = _client.Receive(ref ep);
            string     msg      = Encoding.ASCII.GetString(received);

            SendData data = SendData.MakeJsonFrom(msg);
            // EventHandler(data);
            _receiverSubject.OnNext(data);
        }
    }

    /// <summary>
    /// サーバーから切断する
    /// </summary>
    private static void Disconnect()
    {
        if (!_isConnected) return;

        SendData data = new SendData(EventType.Disconnect)
        {
            Self = new PlayerData
            {
                Uuid = SelfPlayerData.Uuid
            }
        };

        Emit(data);
        Init();
        _client?.Close();
        _thread?.Abort();

        _client = null;
    }

    /// <summary>
    /// サーバーにデータを送信する
    /// </summary>
    /// <param name="data">送信データ</param>
    public static void Emit(SendData data)
    {
        if (!_isConnected) return;

        string msg      = SendData.ParseSendData(data);
        byte[] sendData = Encoding.ASCII.GetBytes(msg);
        _client.Send(sendData, sendData.Length);
    }

    /// <summary>
    /// 受け取ったデータのイベントに応じて処理を行う
    /// </summary>
    /// <param name="data">受信データ</param>
    private static void EventHandler(SendData data)
    {
        EventType type = (EventType) Enum.Parse(typeof(EventType), data.Type);

        switch (type)
        {
            case EventType.Init:
                break;

            case EventType.Join:
                break;

            case EventType.Move:
                break;

            case EventType.Disconnect:
                break;

            case EventType.ClientReload:
                break;

            default:
                Debug.LogError($"イベントタイプ「{nameof(type)}」の処理がありません");

                break;
        }
    }
}
