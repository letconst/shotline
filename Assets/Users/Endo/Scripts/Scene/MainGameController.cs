using System;
using Cinemachine;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MainGameController : SingletonMonoBehaviour<MainGameController>
{
    [SerializeField, Header("生成する対戦相手プレハブ")]
    private GameObject rivalPrefab;

    [SerializeField, Header("初期位置用Virtual CameraのTransform")]
    private Transform vcam1Trf;

    [SerializeField, Header("プレイヤー追従用Virtual CameraのTransform")]
    private Transform vcam2Trf;

    [SerializeField, Header("プレイヤー追従用Virtual Camera")]
    private CinemachineVirtualCamera vcam2;

    [SerializeField, Header("CM BlendListオブジェクト")]
    private GameObject cmBlendListObject;

    private GameObject _playerObject;
    private GameObject _rivalObject;
    private GameObject _inputBlocker;
    private Text       _statusText;
    private Text       _roundText;

    private const string ConnectionWaitingText = "他のプレイヤーを待っています…";
    private const string RivalDisconnected     = "対戦相手が切断しました";

    private IDisposable _receiver;
    private bool        _isClicked;

    public static bool       isControllable;           // 操作可能状態か
    public static bool       isChangeableSceneToTitle; // タイトルに遷移可能な状態か
    public static GameObject linePrefab;               // 射線プレハブ（1Pか2Pかで変動）
    public static GameObject bulletPrefab;             // 弾プレハブ（同上）
    public static GameObject rivalBulletPrefab;

    private CinemachineTransposer      _vcam2Transposer;
    private CinemachineBlendListCamera _cmBlendList;

    protected override void Awake()
    {
        base.Awake();

        _vcam2Transposer         = vcam2.GetCinemachineComponent<CinemachineTransposer>();
        _cmBlendList             = cmBlendListObject.GetComponent<CinemachineBlendListCamera>();
        isChangeableSceneToTitle = false;

        if (NetworkManager.IsConnected)
        {
            cmBlendListObject.SetActive(false);

            // 開始直後は操作不能に（他プレイヤー待機のため）
            isControllable = false;
            MainGameProperty.InputBlocker.SetActive(true);
            SystemProperty.StatusText.text = ConnectionWaitingText;
        }
        else
        {
            isControllable = true;
        }

        _playerObject = GameObject.FindGameObjectWithTag("Player");
        _playerObject.SetActive(false);

        // 1P設定
        if (NetworkManager.IsOwner)
        {
            // プレイヤー追従用カメラを反転
            vcam2Trf.RotateAround(_playerObject.transform.position, Vector3.up, 180);

            // 射線ゲージも逆になるため反転
            MainGameProperty.Instance.LineGaugeObject.transform.Rotate(Vector3.forward, 180);

            // 初期位置設定
            _playerObject.transform.position = MainGameProperty.Instance.startPos1P.position;

            // 相手オブジェクト生成
            _rivalObject = Instantiate(rivalPrefab, MainGameProperty.Instance.startPos2P.position, Quaternion.identity);

            // 表示オブジェクト選択
            _playerObject.transform.Find("player_2").gameObject.SetActive(false);
            _rivalObject.transform.Find("player_1").gameObject.SetActive(false);

            linePrefab        = Resources.Load<GameObject>("Prefabs/Line_PL1");
            bulletPrefab      = Resources.Load<GameObject>("Prefabs/Bullet_PL1");
            rivalBulletPrefab = Resources.Load<GameObject>("Prefabs/Bullet_PL2");
        }
        // 2P設定
        else
        {
            // 初期位置用カメラおよび追従用カメラのオフセット値を反転
            Camera.main.transform.RotateAround(_playerObject.transform.position, Vector3.up, 180);
            vcam1Trf.RotateAround(_playerObject.transform.position, Vector3.up, 180);
            _vcam2Transposer.m_FollowOffset.z *= -1;

            // 初期位置設定
            _playerObject.transform.position = MainGameProperty.Instance.startPos2P.position;

            // 相手オブジェクト生成
            _rivalObject = Instantiate(rivalPrefab, MainGameProperty.Instance.startPos1P.position, Quaternion.identity);

            // 表示オブジェクト選択
            _playerObject.transform.Find("player_1").gameObject.SetActive(false);
            _rivalObject.transform.Find("player_2").gameObject.SetActive(false);

            linePrefab        = Resources.Load<GameObject>("Prefabs/Line_PL2");
            bulletPrefab      = Resources.Load<GameObject>("Prefabs/Bullet_PL2");
            rivalBulletPrefab = Resources.Load<GameObject>("Prefabs/Bullet_PL1");
        }

        _playerObject.SetActive(true);
    }

    private async void Start()
    {
        _inputBlocker = MainGameProperty.InputBlocker;
        _statusText   = SystemProperty.StatusText;
        _roundText    = RoundManager.RoundText;

        if (!NetworkManager.IsConnected) return;

        _receiver = NetworkManager.OnReceived
                                  ?.ObserveOnMainThread()
                                  .Subscribe(OnReceived)
                                  .AddTo(this);

        // 2Pが参加した瞬間に始まるため、少し遅延させる
        await UniTask.Delay(TimeSpan.FromSeconds(2));

        var joinedReq = new JoinedRequest();

        NetworkManager.Emit(joinedReq);

        // ホストならアイテム生成情報送信
        if (NetworkManager.IsOwner)
        {
            var itemInitReq = new ItemInitRequest();

            NetworkManager.Emit(itemInitReq);
        }
    }

    private void Update()
    {
        _isClicked = false;

        if (!isChangeableSceneToTitle) return;

        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                _isClicked = true;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            _isClicked = true;
        }

        // タイトルへの遷移フラグがあり、タッチされたらタイトルへ遷移する
        if (_isClicked)
        {
            NetworkManager.Disconnect();
            SystemSceneManager.LoadNextScene("Title", SceneTransition.Fade);
            _statusText.text         = "";
            isChangeableSceneToTitle = false;
        }
    }

    private async void OnReceived(object res)
    {
        var @base = (RequestBase) res;
        var type  = (EventType) Enum.Parse(typeof(EventType), @base.Type);

        switch (type)
        {
            // 全プレイヤー参加時
            case EventType.Joined:
            {
                _statusText.text = "";
                _roundText.text  = "Game Start";
                cmBlendListObject.SetActive(true);

                // カメラ演出が始まるまで待機
                await UniTask.WaitUntil(() => _cmBlendList.IsBlending);

                UniTask fade           = FadeTransition.FadeIn(_roundText, .5f);
                UniTask cameraBlending = UniTask.WaitWhile(() => _cmBlendList.IsBlending);

                // ラウンドテキスト表示とカメラ演出完了を待機
                await UniTask.WhenAll(fade, cameraBlending);

                // ラウンドテキストフェードアウトと同時に操作可能にするため、並列で
                FadeTransition.FadeOut(_roundText, .5f);

                isControllable = true;
                MainGameProperty.InputBlocker.SetActive(false);

                // ホストならラウンド開始通知
                if (NetworkManager.IsOwner)
                {
                    var roundStartReq = new RequestBase(EventType.RoundStart);

                    NetworkManager.Emit(roundStartReq);
                }

                break;
            }

            // 相手の移動時
            case EventType.PlayerMove:
            {
                var innerRes = (PlayerMoveRequest) res;
                _rivalObject.transform.position = innerRes.Position;
                _rivalObject.transform.rotation = innerRes.Rotation;

                break;
            }

            // アイテム生成通信受信時
            case EventType.ItemGenerate:
            {
                var innerRes = (ItemGenerateRequest) res;

                // ラウンド進行中は生成しない
                if (RoundManager.RoundMove) return;

                // 乱数シードをセットし、アイテムをランダム生成
                Random.InitState(innerRes.Seed);
                ItemManager.GenerateRandomItem();

                break;
            }

            // 相手のアイテム取得時
            case EventType.ItemGet:
            {
                var innerRes = (ItemGetRequest) res;

                // 対象のアイテムオブジェクトを破棄
                GameObject target = MainGameProperty.ItemSpawnPoints[innerRes.GeneratedPointIndex].itemObject;
                ItemManager.DestroyItem(target);

                break;
            }

            // 相手のオブジェクト生成時
            case EventType.Instantiate:
            {
                var innerRes = (InstantiateRequest) res;
                NetworkManager.SyncInstantiate(innerRes.PrefabName, innerRes.Position, innerRes.Rotation,
                                               innerRes.ObjectGuid);

                break;
            }

            // 相手のオブジェクト破棄時
            case EventType.Destroy:
            {
                var innerRes = (DestroyRequest) res;
                NetworkManager.SyncDestroy(innerRes.ObjectGuid);

                break;
            }

            case EventType.RoundUpdate:
            {
                var innerRes = (RoundUpdateRequest) res;

                // 相手の敗北なら勝利表示
                if (innerRes.IsLoseRival && innerRes.RivalUuid != SelfPlayerData.Uuid)
                {
                    Time.timeScale   = .1f;
                    isControllable   = false;
                    _roundText.text  = "Win!";
                    _statusText.text = "タップでタイトルに戻る";

                    isChangeableSceneToTitle = true;
                    _inputBlocker.SetActive(true);

                    await FadeTransition.FadeIn(_roundText, .1f);
                }
                // 攻撃者のラウンド進行が終わり次第、操作可能に
                // 攻撃者側も共通に処理
                else if (innerRes.IsReadyAttackedRival)
                {
                    _statusText.text = "";

                    await FadeTransition.FadeOut(_roundText, .5f);

                    Time.timeScale             = 1;
                    isControllable             = true;
                    PlayerController.isDamaged = false;
                    RoundManager.RoundMove     = false;
                    _roundText.text            = "";
                    _inputBlocker.SetActive(false);

                    var roundStartReq = new RequestBase(EventType.RoundStart);

                    NetworkManager.Emit(roundStartReq);
                }
                // 攻撃者のラウンド進行
                else if (!PlayerController.isDamaged)
                {
                    // ゲーム速度を低速にし、被弾させた表示
                    Time.timeScale  = .1f;
                    _roundText.text = "Hit!";
                    isControllable  = false;
                    _inputBlocker.SetActive(true);

                    // フェード処理
                    await FadeTransition.FadeIn(_roundText, .1f);
                    await UniTask.Delay(TimeSpan.FromSeconds(1), true);
                    await FadeTransition.FadeOut(SystemProperty.FadeCanvasGroup, .5f);

                    _roundText.text = "";
                    Time.timeScale  = 1;

                    // リセット処理
                    ResetPlayersPosition();
                    ShotLineUtil.FreeLineData(ShotLineDrawer.DrawingData);
                    ItemManager.ClearGeneratedItem();
                    Projectile.DestroyAllBullets();

                    _roundText.text  = $"Round {RoundManager.CurrentRound.ToString()}";
                    _statusText.text = "待機中…";

                    await FadeTransition.FadeIn(SystemProperty.FadeCanvasGroup, .5f);

                    // 攻撃者準備完了を送信
                    var roundUpdateReq = new RoundUpdateRequest(true);

                    NetworkManager.Emit(roundUpdateReq);
                }

                break;
            }

            case EventType.Refresh:
            {
                // TODO: UI表示 && 状況に応じてタイトルに戻る
                _statusText.text         = RivalDisconnected;
                isChangeableSceneToTitle = true;

                break;
            }
        }
    }

    /// <summary>
    /// 各プレイヤーの座標をリセットする
    /// TODO: ランダムな対角位置に設定する
    /// </summary>
    public void ResetPlayersPosition()
    {
        _playerObject.SetActive(false);
        _rivalObject.SetActive(false);

        if (NetworkManager.IsOwner)
        {
            _playerObject.transform.position = MainGameProperty.Instance.startPos1P.position;
            _rivalObject.transform.position  = MainGameProperty.Instance.startPos2P.position;
        }
        else
        {
            _playerObject.transform.position = MainGameProperty.Instance.startPos2P.position;
            _rivalObject.transform.position  = MainGameProperty.Instance.startPos1P.position;
        }

        _playerObject.SetActive(true);
        _rivalObject.SetActive(true);
    }
}
