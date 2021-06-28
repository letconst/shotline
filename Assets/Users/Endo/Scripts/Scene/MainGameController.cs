using System;
using UniRx;
using UnityEngine;

public class MainGameController : MonoBehaviour
{
    private const string ConnectionWaitingText = "他のプレイヤーを待っています…";

    private IDisposable _receiver;

    public static bool IsControllable;

    private void Awake()
    {
        IsControllable = true;
        MainGameProperty.InputBlocker.SetActive(true);
        MainGameProperty.StatusText.text = ConnectionWaitingText;

        Transform player = GameObject.FindGameObjectWithTag("Player").transform;

        // 初期位置設定
        if (NetworkManager.IsOwner)
        {
            player.position = MainGameProperty.Instance.startPos1P.position;
        }
        // 2pはカメラを反転させる
        else
        {
            Camera.main.transform.RotateAround(player.position, Vector3.up, 180);
            player.position = MainGameProperty.Instance.startPos2P.position;
        }
    }

    private void Start()
    {
        _receiver = NetworkManager.OnReceived
                                  ?.ObserveOnMainThread()
                                  .Subscribe(OnReceived)
                                  .AddTo(this);

        var data = new SendData(EventType.Joined)
        {
            Self = new PlayerData
            {
                Uuid = SelfPlayerData.Uuid
            }
        };

        NetworkManager.Emit(data);
    }

    private static void OnReceived(SendData data)
    {
        var type = (EventType) Enum.Parse(typeof(EventType), data.Type);

        switch (type)
        {
            case EventType.Joined:
            {
                IsControllable = true;
                MainGameProperty.InputBlocker.SetActive(false);
                MainGameProperty.StatusText.text = "";

                break;
            }

            case EventType.Move:
            {
                break;
            }

            case EventType.Refresh:
            {
                // TODO: UI表示 && 状況に応じてタイトルに戻る
                break;
            }

            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
