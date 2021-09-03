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

    [SerializeField, Header("ルームボタンのプレハブ")]
    private GameObject roomBtnPrefab;

    [SerializeField]
    private Transform layoutParentTrf;

    [SerializeField]
    private Transform createRoomBtnTrf;

    private List<RoomData> _roomButtons;

    private Text _statusText;

    protected override void Awake()
    {
        base.Awake();

        // ボタンイベント登録
        backBtn.onClick.AddListener(OnClickBack);
        refreshBtn.onClick.AddListener(OnClickRefresh);
        _roomButtons = new List<RoomData>();
        _statusText  = SystemProperty.StatusText;

        NetworkManager.OnReceived
                      ?.ObserveOnMainThread()
                      .Subscribe(OnReceived)
                      .AddTo(this);
    }

    private void Start()
    {
        FetchAllRooms();
    }

    public RoomData GetRoomData(RoomEntryButton btn)
    {
        return _roomButtons.FirstOrDefault(button => button.Instance == btn);
    }

    private async void FetchAllRooms()
    {
        var req = new GetAllRoomRequest();

        if (!NetworkManager.IsConnected)
        {
            await NetworkManager.Connect();
        }

        NetworkManager.Emit(req);

        // TODO: 通信中UI表示 && 操作不能
    }

    private void OnClickBack()
    {
        SystemSceneManager.LoadNextScene("Title", SceneTransition.Fade);
    }

    /// <summary>
    /// サーバーへ全部屋の情報取得をリクエストする
    /// </summary>
    private void OnClickRefresh()
    {
        FetchAllRooms();
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
                createRoomBtnTrf.SetAsLastSibling();

                // TODO: 通信中UI非表示 && 操作可能

                // 受け取ったルーム情報のボタンを生成・更新する
                void UpdateButtons()
                {
                    foreach (Room room in innerRes.Rooms)
                    {
                        // 生成済みのルームボタンがあれば内容だけ更新
                        foreach (RoomData button in _roomButtons)
                        {
                            if (button.RoomUuid != room.uuid) continue;

                            button.Instance.UpdateContent(room.clients.Length, room.isInBattle);

                            return;
                        }

                        // なければ新規生成
                        InstantiateRoomButton(room.uuid, room.clients.Length, room.isInBattle);
                    }
                }

                break;
            }

            case EventType.JoinRoom:
            {
                var innerRes = (JoinRoomRequest) res;

                await UniTask.SwitchToMainThread();

                if (innerRes.IsJoinable)
                {
                    SelfPlayerData.PlayerUuid = innerRes.Client.uuid;
                    SelfPlayerData.RoomUuid   = innerRes.RoomUuid;

                    _statusText.text = "マッチング中…";

                    // TODO: 武器選択画面に遷移
                }
                else
                {
                    // TODO: innerRes.Messageをウィンドウ表示 && 閉じた際にリフレッシュ
                    Debug.Log($"入れなかったよ: {innerRes.Message}");
                }

                break;
            }

            case EventType.MatchComplete:
            {
                await UniTask.SwitchToMainThread();

                _statusText.text = "ロード中…";

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

        newBtn.UpdateContent(playerCount, isInBattle);

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
