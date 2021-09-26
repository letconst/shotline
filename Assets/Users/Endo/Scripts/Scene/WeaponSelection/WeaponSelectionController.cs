using Cysharp.Threading.Tasks;
using UniRx;

public class WeaponSelectionController : SingletonMonoBehaviour<WeaponSelectionController>
{
    private TimerScript        _timer;
    private ScrollSnapSelector _snapSelector;

    private bool _isSelected;

    protected override void Awake()
    {
        base.Awake();

        _timer        = WeaponSelectionProperty.Timer;
        _snapSelector = WeaponSelectionProperty.SnapSelector;

        string roomId = SelfPlayerData.RoomUuid;
        WeaponSelectionProperty.RoomIdText.text = $"RoomID: {roomId.Substring(roomId.Length - 4)}";
        WeaponSelectionProperty.ExitButton.onClick.AddListener(OnClickExit);
        WeaponSelectionProperty.ConfirmButton.onClick.AddListener(OnClickConfirm);
        _timer.gameObject.SetActive(false);

        // 武器選択開始リクエスト
        var enterReq = new InRoomRequestBase(EventType.EnterRoom);

        NetworkManager.Emit(enterReq);
    }

    private void Start()
    {
        NetworkManager.OnReceived
                      ?.ObserveOnMainThread()
                      .Subscribe(OnReceived)
                      .AddTo(this);
    }

    /// <summary>
    /// 退出ボタン押下時の処理
    /// </summary>
    private static void OnClickExit()
    {
        SystemUIManager.OpenConfirmWindow("Information", "退出してもよろしいですか？", result =>
        {
            // OK押下時のみ処理
            if (!result) return;

            SystemUIManager.ShowConnectingStatus();

            // 退出リクエスト
            var exitReq = new ExitRoomRequest();

            NetworkManager.Emit(exitReq);
        });
    }

    /// <summary>
    /// 確定ボタン押下時の処理
    /// </summary>
    private void OnClickConfirm()
    {
        // 選択完了後は反応させない
        if (_isSelected) return;

        WeaponDatas selectedWeapon = WeaponManager.weaponDatas[_snapSelector.hIndex - 1];

        SystemUIManager.OpenConfirmWindow("Information", $"{selectedWeapon.WeaponName}でよろしいですか？", result =>
        {
            // OK押下時のみ処理
            if (!result) return;

            SystemUIManager.ShowStatusText("相手の選択を待機中", withShadow: true);
            _timer.gameObject.SetActive(false);
            _isSelected = true;

            ChoiceSelectedWeapon();
        });
    }

    /// <summary>
    /// 選択中の武器に決定する
    /// </summary>
    public void ChoiceSelectedWeapon()
    {
        // 選択武器を保存
        WeaponDatas selectedWeapon = WeaponManager.weaponDatas[_snapSelector.hIndex - 1];
        WeaponManager.SelectWeapon = selectedWeapon;

        // 選択完了リクエスト
        var selectedReq = new InRoomRequestBase(EventType.WeaponSelected);

        NetworkManager.Emit(selectedReq);
    }

    private async void OnReceived(object res)
    {
        var @base = (RequestBase) res;
        var type  = (EventType) System.Enum.Parse(typeof(EventType), @base.Type);

        switch (type)
        {
            // 退出レスポンス
            case EventType.ExitRoom:
            {
                var innerRes = (ExitRoomRequest) res;

                await UniTask.SwitchToMainThread();

                SystemUIManager.HideStatusText();

                // 相手の退出ならタイマー非表示
                if (innerRes.ClientUuid != SelfPlayerData.PlayerUuid)
                {
                    _timer.gameObject.SetActive(false);

                    return;
                }

                // 接続情報破棄
                SelfPlayerData.PlayerUuid = null;
                SelfPlayerData.RoomUuid   = null;

                // 正常に退出可能ならルーム選択へ
                if (innerRes.IsExitable)
                {
                    SystemSceneManager.LoadNextScene("RoomSelection", SceneTransition.Fade);
                }
                // エラーがある場合は内容も表示
                else
                {
                    SystemUIManager.OpenAlertWindow("Error", innerRes.Message,
                                                    () =>
                                                    {
                                                        SystemSceneManager.LoadNextScene(
                                                            "RoomSelection", SceneTransition.Fade);
                                                    });
                }

                break;
            }

            // 全員の武器選択開始レスポンス
            case EventType.EnterRoom:
            {
                // 武器選択済みなら処理しない
                if (_isSelected) break;

                await UniTask.SwitchToMainThread();

                // タイマー起動
                _timer.gameObject.SetActive(true);
                _timer.ResetTimer();

                break;
            }

            // 全員の武器選択完了レスポンス
            case EventType.WeaponSelected:
            {
                await UniTask.SwitchToMainThread();

                SystemUIManager.HideStatusText();
                SystemSceneManager.LoadNextScene("MainGameScene", SceneTransition.Fade, isShowStatus: true);

                break;
            }

            // 相手切断時
            case EventType.Refresh:
            {
                await UniTask.SwitchToMainThread();

                _timer.gameObject.SetActive(false);

                break;
            }

            case EventType.Error:
            {
                var innerRes = (ErrorRequest) res;

                await UniTask.SwitchToMainThread();

                SystemUIManager.HideStatusText();
                SystemUIManager.OpenAlertWindow("Error", innerRes.Message,
                                                () =>
                                                {
                                                    NetworkManager.Disconnect();
                                                    SystemSceneManager.LoadNextScene("Title", SceneTransition.Fade);
                                                });

                break;
            }
        }
    }
}
