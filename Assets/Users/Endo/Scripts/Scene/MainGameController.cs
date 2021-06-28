using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class MainGameController : MonoBehaviour
{
    [SerializeField, Header("生成する対戦相手オブジェクト")]
    private GameObject rivalPrefab;

    private Transform  _playerTrf;
    private GameObject _rivalObject;

    private const string ConnectionWaitingText = "他のプレイヤーを待っています…";
    private const string RivalDisconnected     = "対戦相手が切断しました";

    private IDisposable _receiver;

    public static bool IsControllable;

    private void Awake()
    {
        // 開始直後は操作不能に（他プレイヤー待機のため）
        IsControllable = true;
        MainGameProperty.InputBlocker.SetActive(true);
        MainGameProperty.StatusText.text = ConnectionWaitingText;

        _playerTrf = GameObject.FindGameObjectWithTag("Player").transform;
        _playerTrf.gameObject.SetActive(false);

        // 初期位置設定
        if (NetworkManager.IsOwner)
        {
            _playerTrf.position = MainGameProperty.Instance.startPos1P.position;

            _rivalObject = Instantiate(rivalPrefab, MainGameProperty.Instance.startPos2P.position, Quaternion.identity);
        }
        // 2pはカメラを反転させる
        else
        {
            Camera.main.transform.RotateAround(_playerTrf.position, Vector3.up, 180);
            _playerTrf.position = MainGameProperty.Instance.startPos2P.position;

            _rivalObject = Instantiate(rivalPrefab, MainGameProperty.Instance.startPos1P.position, Quaternion.identity);
        }

        _playerTrf.gameObject.SetActive(true);
    }

    private async void Start()
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

        // 2Pが参加した瞬間に始まるため、少し遅延させる
        await UniTask.Delay(TimeSpan.FromSeconds(2));

        NetworkManager.Emit(data);
    }

    private void OnReceived(SendData data)
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

            case EventType.PlayerMove:
            {
                _rivalObject.transform.position = data.Rival.Position;
                _rivalObject.transform.rotation = data.Rival.Rotation;

                break;
            }

            case EventType.Refresh:
            {
                // TODO: UI表示 && 状況に応じてタイトルに戻る
                MainGameProperty.StatusText.text = RivalDisconnected;

                break;
            }

            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
