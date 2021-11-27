using System;
using Cinemachine;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

public class MainGameController : SingletonMonoBehaviour<MainGameController>, IManagedMethod
{
    private GameObject _playerObject;

    private IDisposable _receiver;
    private bool        _isClicked;

    public static bool       isControllable;           // 操作可能状態か
    public static bool       isChangeableSceneToTitle; // タイトルに遷移可能な状態か
    public static GameObject linePrefab;               // 射線プレハブ（1Pか2Pかで変動）
    public static GameObject bulletPrefab;             // 弾プレハブ（同上）
    public static GameObject rivalBulletPrefab;
    public static Transform  rivalTrf;
    public static GameObject bulletCollideParticle;
    public static GameObject rivalBulletCollideParticle;

    private GameObject                 _cmBlendListObject;
    private CinemachineBlendListCamera _cmBlendList;

    public async void ManagedStart()
    {
        SoundManager.Instance.PlayBGM(BGMLabel.MainGame);

        _playerObject            = GameObject.FindGameObjectWithTag("Player");
        _cmBlendListObject       = MainGameProperty.Instance.CmBlendListObject;
        _cmBlendList             = _cmBlendListObject.GetComponent<CinemachineBlendListCamera>();
        isChangeableSceneToTitle = false;

        if (NetworkManager.IsConnected)
        {
            _cmBlendListObject.SetActive(false);

            // 開始直後は操作不能に（他プレイヤー待機のため）
            isControllable = false;
        }
        else
        {
            isControllable = true;
        }

        if (!NetworkManager.IsConnected) return;

        SystemUIManager.SetInputBlockerVisibility(true);
        SystemUIManager.ShowStatusText(StatusText.NowWaitingOther);

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

    public void ManagedUpdate()
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
            SystemUIManager.HideStatusText();
            isChangeableSceneToTitle = false;
        }
    }

    private async void OnReceived(object res)
    {
        var @base = (InRoomRequestBase) res;
        var type  = (EventType) Enum.Parse(typeof(EventType), @base.Type);

        switch (type)
        {
            // 全プレイヤー参加時
            case EventType.Joined:
            {
                SystemUIManager.HideStatusText();

                _cmBlendListObject.SetActive(true);

                // カメラ演出が始まるまで待機
                await UniTask.WaitUntil(() => _cmBlendList.IsBlending);

                UniTask fade           = RoundManager.RoundInitFade();
                UniTask cameraBlending = UniTask.WaitWhile(() => _cmBlendList.IsBlending);

                // ラウンドテキスト表示とカメラ演出完了を待機
                await UniTask.WhenAll(fade, cameraBlending);

                // ラウンドテキストフェードアウトと同時に操作可能にするため、並列で
                FadeTransition.FadeOut(MainGameProperty.RoundTitleImg, .5f);
                FadeTransition.FadeOut(MainGameProperty.PlayerPointsCanvasGroup, .5f);

                isControllable = true;
                SystemUIManager.SetInputBlockerVisibility(false);

                // ホストならラウンド開始通知
                if (NetworkManager.IsOwner)
                {
                    var roundStartReq = new RoundStartRequest();

                    NetworkManager.Emit(roundStartReq);
                }

                break;
            }

            // 相手の移動時
            case EventType.PlayerMove:
            {
                var innerRes = (PlayerMoveRequest) res;
                rivalTrf.position = innerRes.Position;
                rivalTrf.rotation = innerRes.Rotation;

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
                ItemManager.GenerateRandomItem(innerRes.Seed);

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
                if (innerRes.IsLoseRival && innerRes.RivalUuid != SelfPlayerData.PlayerUuid)
                {
                    await RoundManager.ShowBattleResult(ResultType.Win);
                }
                // 攻撃者のラウンド進行が終わり次第、操作可能に
                // 攻撃者側も共通に処理
                else if (innerRes.IsReadyAttackedRival)
                {
                    SystemUIManager.HideStatusText();
                    await RoundManager.RoundStartFadeOut();

                    var roundStartReq = new RoundStartRequest();

                    NetworkManager.Emit(roundStartReq);
                }
                // 攻撃者のラウンド進行
                else if (!PlayerController.isDamaged)
                {
                    RoundManager.Instance.playerPoint++;
                    await RoundManager.RoundUpdateFadeOut(JudgeType.Hit);

                    // リセット処理
                    ResetPlayersPosition();
                    ShotLineUtil.FreeLineData(ShotLineDrawer.DrawingData);
                    ItemManager.ClearGeneratedItem();
                    Projectile.DestroyAllBullets();
                    MainGameProperty.SuddenDeathImg.color = new Color(1, 1, 1, 0);

                    // 外壁サイズリセット
                    foreach (Transform wall in MainGameProperty.SotoWalls)
                    {
                        wall.localScale = Vector3.one;
                    }

                    RoundManager.Instance.SuddenDeathFlag = false;
                    RoundManager.Instance.ResetCount();
                    SystemUIManager.ShowConnectingStatus();
                    await RoundManager.RoundUpdateFadeIn();

                    // 攻撃者準備完了を送信
                    var roundUpdateReq = new RoundUpdateRequest(true);

                    NetworkManager.Emit(roundUpdateReq);
                }

                break;
            }

            case EventType.Refresh:
            {
                SystemUIManager.HideStatusText();
                SystemUIManager.OpenAlertWindow("Information", "対戦相手が切断しました。タイトルに戻ります。",
                                                () =>
                                                {
                                                    NetworkManager.Disconnect();
                                                    SystemSceneManager.LoadNextScene("Title", SceneTransition.Fade);
                                                });

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
        rivalTrf.gameObject.SetActive(false);

        if (NetworkManager.IsOwner)
        {
            _playerObject.transform.position = MainGameProperty.Instance.startPos1P.position;
            rivalTrf.position                = MainGameProperty.Instance.startPos2P.position;
        }
        else
        {
            _playerObject.transform.position = MainGameProperty.Instance.startPos2P.position;
            rivalTrf.position                = MainGameProperty.Instance.startPos1P.position;
        }

        _playerObject.SetActive(true);
        rivalTrf.gameObject.SetActive(true);
    }
}
