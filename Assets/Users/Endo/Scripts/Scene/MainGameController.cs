﻿using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class MainGameController : MonoBehaviour
{
    [SerializeField, Header("生成する対戦相手オブジェクト")]
    private GameObject rivalPrefab;

    private GameObject _playerObject;
    private GameObject _rivalObject;
    private GameObject _inputBlocker;
    private Text       _statusText;
    private Text       _roundText;

    private const string ConnectionWaitingText = "他のプレイヤーを待っています…";
    private const string RivalDisconnected     = "対戦相手が切断しました";

    private IDisposable _receiver;

    public static bool       isControllable; // 操作可能状態か
    public static GameObject linePrefab;     // 射線プレハブ（1Pか2Pかで変動）
    public static GameObject bulletPrefab;   // 弾プレハブ（同上）
    public static GameObject rivalBulletPrefab;

    private void Awake()
    {
        if (NetworkManager.IsConnected)
        {
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
            _playerObject.transform.position = MainGameProperty.Instance.startPos1P.position;

            _rivalObject = Instantiate(rivalPrefab, MainGameProperty.Instance.startPos2P.position, Quaternion.identity);
            _rivalObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/Bullet_PL2");

            linePrefab        = Resources.Load<GameObject>("Prefabs/Line_PL1");
            bulletPrefab      = Resources.Load<GameObject>("Prefabs/Bullet_PL1");
            rivalBulletPrefab = Resources.Load<GameObject>("Prefabs/Bullet_PL2");
        }
        // 2P設定
        else
        {
            // 2Pはカメラ反転
            Camera.main.transform.RotateAround(_playerObject.transform.position, Vector3.up, 180);
            _playerObject.transform.position = MainGameProperty.Instance.startPos2P.position;

            _rivalObject = Instantiate(rivalPrefab, MainGameProperty.Instance.startPos1P.position, Quaternion.identity);
            _rivalObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/Bullet_PL1");

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

        var data = new SendData(EventType.Joined)
        {
            Self = new PlayerData()
        };

        // 2Pが参加した瞬間に始まるため、少し遅延させる
        await UniTask.Delay(TimeSpan.FromSeconds(2));

        NetworkManager.Emit(data);
    }

    private async void OnReceived(SendData data)
    {
        var type = (EventType) Enum.Parse(typeof(EventType), data.Type);

        switch (type)
        {
            case EventType.Joined:
            {
                isControllable = true;
                MainGameProperty.InputBlocker.SetActive(false);
                _statusText.text = "";

                break;
            }

            case EventType.PlayerMove:
            {
                _rivalObject.transform.position = data.Rival.Position;
                _rivalObject.transform.rotation = data.Rival.Rotation;

                break;
            }

            case EventType.RoundUpdate:
            {
                // 相手の敗北なら勝利表示
                if (data.Rival.isLose && data.Rival.Uuid != SelfPlayerData.Uuid)
                {
                    Time.timeScale  = .1f;
                    isControllable  = false;
                    _roundText.text = "Win!";
                    _inputBlocker.SetActive(true);

                    await FadeTransition.FadeIn(_roundText, .1f);
                }
                // 攻撃者のラウンド進行が終わり次第、操作可能に
                // 攻撃者側も共通に処理
                else if (data.isReadyAttacker)
                {
                    _statusText.text = "";

                    await FadeTransition.FadeOut(_roundText, .5f);

                    Time.timeScale             = 1;
                    isControllable             = true;
                    PlayerController.isDamaged = false;
                    RoundManager.RoundMove     = false;
                    _roundText.text            = "";
                    _inputBlocker.SetActive(false);
                }
                // 攻撃者のラウンド進行
                else if (!PlayerController.isDamaged)
                {
                    Time.timeScale  = .1f;
                    _roundText.text = "Hit!";
                    isControllable  = false;
                    _inputBlocker.SetActive(true);

                    await FadeTransition.FadeIn(_roundText, .1f);
                    await UniTask.Delay(TimeSpan.FromSeconds(.5f), true);
                    await FadeTransition.FadeOut(SystemProperty.FadeCanvasGroup, .5f);

                    _roundText.text = "";
                    Time.timeScale  = 1;

                    ResetPlayersPosition();
                    ShotLineUtil.FreeLineData(ShotLineDrawer.DrawingData);

                    _roundText.text  = $"Round {RoundManager.CurrentRound.ToString()}";
                    _statusText.text = "待機中…";

                    await FadeTransition.FadeIn(SystemProperty.FadeCanvasGroup, 5f);

                    // 攻撃者準備完了を送信
                    var attackerData = new SendData(EventType.RoundUpdate)
                    {
                        Self            = new PlayerData(),
                        isReadyAttacker = true
                    };

                    NetworkManager.Emit(attackerData);
                }

                break;
            }

            case EventType.Refresh:
            {
                // TODO: UI表示 && 状況に応じてタイトルに戻る
                _statusText.text = RivalDisconnected;

                break;
            }
        }
    }

    private void ResetPlayersPosition()
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
