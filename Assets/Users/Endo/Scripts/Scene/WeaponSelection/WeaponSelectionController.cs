using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class WeaponSelectionController : MonoBehaviour
{
    private void Awake()
    {
        WeaponSelectionProperty.ExitButton.onClick.AddListener(OnClickExit);
        WeaponSelectionProperty.ConfirmButton.onClick.AddListener(OnClickConfirm);
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
        SystemUIManager.ShowConnectingStatus();

        // 接続情報破棄
        SelfPlayerData.PlayerUuid = null;
        SelfPlayerData.RoomUuid   = null;

        // 退出リクエスト
        var exitReq = new ExitRoomRequest();

        NetworkManager.Emit(exitReq);
    }

    /// <summary>
    /// 確定ボタン押下時の処理
    /// </summary>
    private void OnClickConfirm()
    {
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

                // 正常に退出可能ならルーム選択へ
                if (innerRes.IsExitable)
                {
                    SystemSceneManager.LoadNextScene("RoomSelection", SceneTransition.Fade);
                }
                // エラーがある場合は内容も表示
                else
                {
                    SystemUIManager.OpenAlertWindow("エラー", innerRes.Message, () =>
                    {
                        SystemSceneManager.LoadNextScene("RoomSelection", SceneTransition.Fade);
                    });
                }

                break;
            }
        }
    }
}
