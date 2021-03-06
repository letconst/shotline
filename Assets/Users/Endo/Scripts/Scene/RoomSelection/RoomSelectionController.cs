using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class RoomSelectionController : SingletonMonoBehaviour<RoomSelectionController>
{
    [SerializeField, Header("戻るボタン")]
    private Button backBtn;

    [SerializeField, Header("ルーム更新ボタン")]
    private Button refreshBtn;

    [SerializeField, Header("ルーム作成ボタン")]
    private Button createRoomBtn;

    [SerializeField, Header("ルームボタンのプレハブ")]
    private GameObject roomBtnPrefab;

    [SerializeField]
    private Transform layoutParentTrf;

    private Transform _createRoomBtnTrf;

    private List<RoomData> _roomButtons;

    protected override void Awake()
    {
        base.Awake();

        // ボタンイベント登録
        backBtn.onClick.AddListener(OnClickBack);
        refreshBtn.onClick.AddListener(OnClickRefresh);
        createRoomBtn.onClick.AddListener(OnClickCreateRoom);
        _createRoomBtnTrf = createRoomBtn.transform;
        _roomButtons      = new List<RoomData>();
    }

    private void Start()
    {
        FetchAllRooms();

        NetworkManager.OnReceived
                      ?.ObserveOnMainThread()
                      .Subscribe(OnReceived)
                      .AddTo(this);
    }

    public RoomData GetRoomData(RoomEntryButton btn)
    {
        return _roomButtons.FirstOrDefault(button => button.Instance == btn);
    }

    private static async void FetchAllRooms()
    {
        SystemUIManager.ShowConnectingStatus();

        var req = new GetAllRoomRequest();

        if (!NetworkManager.IsConnected)
        {
            await NetworkManager.Connect();
        }

        NetworkManager.Emit(req);
    }

    /// <summary>
    /// 戻るボタン押下時の処理
    /// </summary>
    private static void OnClickBack()
    {
        NetworkManager.Disconnect();
        SystemUIManager.HideStatusText();
        SystemSceneManager.LoadNextScene("Title", SceneTransition.Fade);
    }

    /// <summary>
    /// サーバーへ全部屋の情報取得をリクエストする
    /// </summary>
    private static void OnClickRefresh()
    {
        FetchAllRooms();
    }

    /// <summary>
    /// ルーム作成ボタン押下時の処理
    /// </summary>
    private static void OnClickCreateRoom()
    {
        if (!NetworkManager.IsConnected) return;

        var req = new RequestBase(EventType.CreateRoom);

        NetworkManager.Emit(req);
        SystemUIManager.ShowConnectingStatus();
    }

    private async void OnReceived(object res)
    {
        var @base = (RequestBase) res;
        var type  = (EventType) Enum.Parse(typeof(EventType), @base.Type);

        switch (type)
        {
            case EventType.GetAllRoom:
            {
                var innerRes = (GetAllRoomRequest) res;

                await UniTask.SwitchToMainThread();

                UpdateButtons();

                // ルーム作成ボタンを最下部へ
                _createRoomBtnTrf.SetAsLastSibling();

                // 受け取ったルーム情報のボタンを生成・更新する
                void UpdateButtons()
                {
                    foreach (Room room in innerRes.Rooms)
                    {
                        // 生成済みのルームボタンがあれば内容だけ更新
                        foreach (RoomData button in _roomButtons)
                        {
                            if (button.RoomUuid != room.uuid) continue;

                            button.Instance.UpdateContent(room.clients.Length, room.isInBattle, room.uuid);

                            return;
                        }

                        // なければ新規生成
                        InstantiateRoomButton(room.uuid, room.clients.Length, room.isInBattle);
                    }
                }

                SystemUIManager.HideStatusText();

                break;
            }

            // ルーム参加レスポンス
            case EventType.JoinRoom:
            {
                var innerRes = (JoinRoomRequest) res;

                await UniTask.SwitchToMainThread();

                if (innerRes.IsJoinable)
                {
                    SelfPlayerData.PlayerUuid = innerRes.Client.uuid;
                    SelfPlayerData.RoomUuid   = innerRes.RoomUuid;

                    SystemUIManager.HideStatusText();
                    SystemSceneManager.LoadNextScene("WeaponSelection", SceneTransition.Fade);
                }
                else
                {
                    SystemUIManager.HideStatusText();
                    SystemUIManager.OpenAlertWindow("エラー", innerRes.Message, FetchAllRooms);
                }

                break;
            }

            case EventType.MatchComplete:
            {
                await UniTask.SwitchToMainThread();

                SystemUIManager.ShowStatusText(StatusText.NowLoading);

                SystemSceneManager.LoadNextScene("MainGameScene", SceneTransition.Fade);

                break;
            }
        }
    }

    /// <summary>
    /// ルーム選択ボタンを生成する
    /// </summary>
    /// <param name="roomUuid">ルームのUUID</param>
    /// <param name="playerCount">ルームの参加プレイヤー数</param>
    /// <param name="isInBattle">ルームが対戦中か</param>
    private void InstantiateRoomButton(string roomUuid, int playerCount, bool isInBattle)
    {
        GameObject newBtnObj = Instantiate(roomBtnPrefab, layoutParentTrf);
        var        newBtn    = newBtnObj.GetComponent<RoomEntryButton>();

        newBtn.UpdateContent(playerCount, isInBattle, roomUuid);

        _roomButtons.Add(new RoomData
        {
            RoomUuid    = roomUuid,
            RoomNumber  = (byte) (_roomButtons.Count + 1),
            PlayerCount = (byte) playerCount,
            IsInBattle  = isInBattle,
            Instance    = newBtn
        });
    }
}
