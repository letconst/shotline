using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class MainGameController : MonoBehaviour
{
    [SerializeField, Header("生成する対戦相手オブジェクト")]
    private GameObject rivalPrefab;

    private GameObject _rivalObject;

    private const string ConnectionWaitingText = "他のプレイヤーを待っています…";
    private const string RivalDisconnected     = "対戦相手が切断しました";

    private IDisposable _receiver;

    public static bool       IsControllable; // 操作可能状態か
    public static GameObject LinePrefab;     // 射線プレハブ（1Pか2Pかで変動）
    public static GameObject BulletPrefab;   // 弾プレハブ（同上）
    public static GameObject RivalBulletPrefab;

    private void Awake()
    {
        if (NetworkManager.IsConnected)
        {
            // 開始直後は操作不能に（他プレイヤー待機のため）
            IsControllable = false;
            MainGameProperty.InputBlocker.SetActive(true);
            MainGameProperty.StatusText.text = ConnectionWaitingText;
        }
        else
        {
            IsControllable = true;
        }

        Transform playerTrf = GameObject.FindGameObjectWithTag("Player").transform;
        playerTrf.gameObject.SetActive(false);

        // 1P設定
        if (NetworkManager.IsOwner)
        {
            playerTrf.position = MainGameProperty.Instance.startPos1P.position;

            _rivalObject = Instantiate(rivalPrefab, MainGameProperty.Instance.startPos2P.position, Quaternion.identity);
            _rivalObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/Bullet_PL2");

            LinePrefab        = Resources.Load<GameObject>("Prefabs/Line_PL1");
            BulletPrefab      = Resources.Load<GameObject>("Prefabs/Bullet_PL1");
            RivalBulletPrefab = Resources.Load<GameObject>("Prefabs/Bullet_PL2");
        }
        // 2P設定
        else
        {
            // 2Pはカメラ反転
            Camera.main.transform.RotateAround(playerTrf.position, Vector3.up, 180);
            playerTrf.position = MainGameProperty.Instance.startPos2P.position;

            _rivalObject = Instantiate(rivalPrefab, MainGameProperty.Instance.startPos1P.position, Quaternion.identity);
            _rivalObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/Bullet_PL1");

            LinePrefab        = Resources.Load<GameObject>("Prefabs/Line_PL2");
            BulletPrefab      = Resources.Load<GameObject>("Prefabs/Bullet_PL2");
            RivalBulletPrefab = Resources.Load<GameObject>("Prefabs/Bullet_PL1");
        }

        playerTrf.gameObject.SetActive(true);
    }

    private async void Start()
    {
        if (!NetworkManager.IsConnected) return;

        _receiver = NetworkManager.OnReceived
                                  ?.ObserveOnMainThread()
                                  .Subscribe(OnReceived)
                                  .AddTo(this);

        var data = new SendData(EventType.Joined)
        {
            Self = new PlayerData()
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
        }
    }
}
