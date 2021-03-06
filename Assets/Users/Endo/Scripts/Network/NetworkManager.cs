using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class NetworkManager : SingletonMonoBehaviour<NetworkManager>
{
    [SerializeField, Header("サーバーアドレス")]
    private string address = "localhost";

    [SerializeField, Header("サーバーポート"), Range(0, 65535)]
    private ushort port = 6080;

    [SerializeField, Header("第二サーバーポート"), Range(0, 65535)]
    private ushort secondaryPort = 6081;

    private static UdpClient _client;
    private static Thread    _thread;

    private static Subject<object> _receiverSubject;

    private static Dictionary<string, GameObject> _networkedObjects;

    /// <summary>
    /// 自身がホストか。部屋を立てたプレイヤーならtrueとなる。
    /// </summary>
    public static bool IsOwner { get; private set; }

    public static IObservable<object> OnReceived => _receiverSubject;

    public static bool   IsConnected  { get; private set; } // サーバーに接続中か
    public static string RivalAddress { get; private set; }
    public static int    RivalPort    { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        _receiverSubject       = new Subject<object>();
        SelfPlayerData.Address = address;
        SelfPlayerData.Port    = port;

        Init();
        DontDestroyOnLoad(this);
        OnReceived.Subscribe(EventHandler);
    }

#if UNITY_ANDROID
    private void OnApplicationPause(bool pauseStatus)
    {
        // Android限定
        // アプリを一時的に閉じた際、サーバーからの切断処理をしたいためゲームを終了。本来はタスクキル時に実行したい
        if (pauseStatus)
        {
            Application.Quit();
        }
    }
#endif

    private void OnApplicationQuit()
    {
        Disconnect();
    }

    /// <summary>
    /// 接続情報を初期化する
    /// </summary>
    private static void Init()
    {
        _networkedObjects = new Dictionary<string, GameObject>();

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
            await UniTask.Run(() => _client.Connect(Instance.address, SelfPlayerData.Port));

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
    public static void Disconnect()
    {
        if (!IsConnected) return;

        var disconnectReq = new DisconnectRequest();

        Emit(disconnectReq);
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

                object data = ParseRequest(msg);
                _receiverSubject.OnNext(data);
            }
        }
        catch (SocketException e)
        {
            IsConnected = false;

            // UIを書き換えるためメインスレッドに戻す（暫定）
            await UniTask.SwitchToMainThread();

            bool isClickOk = false;

            SystemUIManager.OpenConfirmWindow("Error", "サーバーに接続できませんでした。再接続しますか？", result =>
            {
                isClickOk = result;

                if (!result) return;

                SelfPlayerData.Port = Instance.port;

                Connect();
            }, () =>
            {
                if (isClickOk) return;

                SystemUIManager.OpenConfirmWindow("Information", "デバッグサーバーに接続しますか？", result =>
                {
                    if (!result) return;

                    SelfPlayerData.Port = Instance.secondaryPort;

                    Connect();
                });
            });

            Debug.LogError(e.Message);
            IsConnected = false;
        }
    }

    /// <summary>
    /// 受信メッセージから適当なリクエストのJSONインスタンスを取得する
    /// </summary>
    /// <param name="msg">受信メッセージ</param>
    /// <returns>JSONインスタンス</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private static object ParseRequest(string msg)
    {
        var data = RequestBase.MakeJsonFrom<RequestBase>(msg);
        var type = (EventType) Enum.Parse(typeof(EventType), data.Type);

        return type switch
        {
            EventType.GetAllRoom       => RequestBase.MakeJsonFrom<GetAllRoomRequest>(msg),
            EventType.JoinRoom         => RequestBase.MakeJsonFrom<JoinRoomRequest>(msg),
            EventType.EnterRoom        => RequestBase.MakeJsonFrom<InRoomRequestBase>(msg),
            EventType.ExitRoom         => RequestBase.MakeJsonFrom<ExitRoomRequest>(msg),
            EventType.WeaponSelected   => RequestBase.MakeJsonFrom<InRoomRequestBase>(msg),
            EventType.MatchComplete    => RequestBase.MakeJsonFrom<MatchCompleteRequest>(msg),
            EventType.Match            => RequestBase.MakeJsonFrom<MatchRequest>(msg),
            EventType.Joined           => RequestBase.MakeJsonFrom<JoinedRequest>(msg),
            EventType.PlayerMove       => RequestBase.MakeJsonFrom<PlayerMoveRequest>(msg),
            EventType.BulletMove       => RequestBase.MakeJsonFrom<BulletMoveRequest>(msg),
            EventType.ItemInit         => RequestBase.MakeJsonFrom<ItemInitRequest>(msg),
            EventType.ItemGenerate     => RequestBase.MakeJsonFrom<ItemGenerateRequest>(msg),
            EventType.ItemGet          => RequestBase.MakeJsonFrom<ItemGetRequest>(msg),
            EventType.Instantiate      => RequestBase.MakeJsonFrom<InstantiateRequest>(msg),
            EventType.Destroy          => RequestBase.MakeJsonFrom<DestroyRequest>(msg),
            EventType.ShieldUpdate     => RequestBase.MakeJsonFrom<ShieldUpdateRequest>(msg),
            EventType.RoundStart       => RequestBase.MakeJsonFrom<RoundStartRequest>(msg),
            EventType.RoundUpdate      => RequestBase.MakeJsonFrom<RoundUpdateRequest>(msg),
            EventType.SuddenDeathStart => RequestBase.MakeJsonFrom<InRoomRequestBase>(msg),
            EventType.Disconnect       => RequestBase.MakeJsonFrom<DisconnectRequest>(msg),
            EventType.Refresh          => RequestBase.MakeJsonFrom<RefreshRequest>(msg),
            EventType.RoomRemoved      => RequestBase.MakeJsonFrom<RequestBase>(msg),
            EventType.Error            => RequestBase.MakeJsonFrom<ErrorRequest>(msg),
            _                          => throw new ArgumentOutOfRangeException()
        };
    }

    /// <summary>
    /// サーバーにデータを送信する
    /// </summary>
    /// <param name="data">送信データ</param>
    public static void Emit(RequestBase data)
    {
        if (!IsConnected) return;

        string msg      = RequestBase.ParseSendData(data);
        byte[] sendData = Encoding.ASCII.GetBytes(msg);
        _client.Send(sendData, sendData.Length);
    }

    /// <summary>
    /// ネットワーク化されたGameObjectをプレハブパスから生成する
    /// </summary>
    /// <param name="prefabPath">生成するプレハブのパス</param>
    /// <param name="position">生成位置</param>
    /// <param name="rotation">回転</param>
    /// <returns>生成されたGameObject</returns>
    public static GameObject Instantiate(string prefabPath, Vector3 position, Quaternion rotation)
    {
        var        prefab = Resources.Load<GameObject>(prefabPath);
        GameObject result = GameObject.Instantiate(prefab, position, rotation);
        string     guid   = Guid.NewGuid().ToString("N");

        result.OnDestroyAsObservable()
              .Subscribe(_ => OnTargetDestroyed(guid))
              .AddTo(result);

        _networkedObjects.Add(guid, result);

        // 生成されたことを他クライアントに通知
        var req = new InstantiateRequest(prefabPath, guid, position, rotation);

        Emit(req);

        return result;
    }

    private static void OnTargetDestroyed(string guid)
    {
        _networkedObjects.Remove(guid);

        var req = new DestroyRequest(guid);

        Emit(req);
    }

    /// <summary>
    /// ネットワーク化されたGameObjectをローカルで生成する（受信時用）
    /// </summary>
    /// <param name="prefabPath">生成するプレハブのパス</param>
    /// <param name="position">生成位置</param>
    /// <param name="rotation">回転</param>
    /// <param name="guid"></param>
    /// <returns>生成されたGameObject</returns>
    public static GameObject SyncInstantiate(string prefabPath, Vector3 position, Quaternion rotation, string guid)
    {
        var        prefab = Resources.Load<GameObject>(prefabPath);
        GameObject result = GameObject.Instantiate(prefab, position, rotation);

        _networkedObjects.Add(guid, result);

        return result;
    }

    /// <summary>
    /// ネットワーク化されたGameObjectをローカルで破棄する（受信時用）
    /// </summary>
    /// <param name="guid"></param>
    public static void SyncDestroy(string guid)
    {
        KeyValuePair<string, GameObject> target;

        foreach (KeyValuePair<string, GameObject> networkedObject in _networkedObjects)
        {
            if (networkedObject.Key != guid) continue;

            target = networkedObject;

            break;
        }

        _networkedObjects.Remove(target.Key);
        Destroy(target.Value);
    }

    /// <summary>
    /// 指定したGameObjectのネットワークGUIDを取得する。ネットワーク化されていなければnullが返る。
    /// </summary>
    /// <param name="target"></param>
    /// <returns>GUID</returns>
    public static string GetGuid(GameObject target)
    {
        string result = null;

        foreach (KeyValuePair<string, GameObject> networkedObject in _networkedObjects)
        {
            if (networkedObject.Value != target) continue;

            result = networkedObject.Key;

            break;
        }

        return result;
    }

    /// <summary>
    /// サーバーからのレスポンスのイベントに応じて処理を行う
    /// </summary>
    /// <param name="res">受信データ</param>
    private static async void EventHandler(object res)
    {
        var tmp  = (RequestBase) res;
        var type = (EventType) Enum.Parse(typeof(EventType), tmp.Type);
        _client.Client.ReceiveTimeout = 0;

        switch (type)
        {
            case EventType.JoinRoom:
            {
                var innerRes = (JoinRoomRequest) res;

                IsOwner = innerRes.IsOwner;

                break;
            }

            case EventType.Error:
            {
                var innerRes = (ErrorRequest) res;
                Debug.LogError(innerRes.Message);

                break;
            }

            case EventType.RoomRemoved:
            {
                await UniTask.SwitchToMainThread();

                SystemUIManager.OpenAlertWindow("Error", "切断されました。", () =>
                {
                    SystemUIManager.HideStatusText();
                    SystemSceneManager.LoadNextScene("Title", SceneTransition.Fade);
                });

                break;
            }
        }
    }
}
