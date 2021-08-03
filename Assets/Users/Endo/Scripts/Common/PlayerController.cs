using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private GameObject _rivalObject;
    private Text       _roundText;
    private Text       _statusText;

    public static bool isDamaged;

    private void Awake()
    {
        isDamaged = false;

        // 1P設定
        if (NetworkManager.IsOwner)
        {
            GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/Bullet_PL1");
        }
        // 2P設定
        else
        {
            GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/Bullet_PL2");
        }

        if (NetworkManager.IsConnected)
        {
            // 座標が更新されたらサーバーに座標更新通信
            transform.ObserveEveryValueChanged(x => x.position)
                     .Subscribe(OnPositionChanged)
                     .AddTo(this);
        }
    }

    private void Start()
    {
        _rivalObject = GameObject.FindGameObjectWithTag("Rival");
        _roundText   = RoundManager.RoundText;
        _statusText  = SystemProperty.StatusText;
    }

    private async void OnTriggerEnter(Collider other)
    {
        // ラウンド進行処理中は被弾しない
        if (RoundManager.RoundMove) return;

        if (RoundManager.CurrentPlayerLife == 0) return;

        if (!other.CompareTag("RivalBullet")) return;

        SoundManager.Instance.PlaySE(SELabel.Damage);
        RoundManager.HitVerification();

        Time.timeScale                    = .1f;
        MainGameController.isControllable = false;
        isDamaged                         = true;
        MainGameProperty.InputBlocker.SetActive(true);

        var roundUpdateReq = new RoundUpdateRequest();

        // 残機ゼロ時
        if (RoundManager.CurrentPlayerLife == 0)
        {
            roundUpdateReq.IsLoseRival = true;
            _roundText.text            = "Lose!";
            _statusText.text           = "タップでタイトルに戻る";

            MainGameController.isChangeableSceneToTitle = true;
            NetworkManager.Emit(roundUpdateReq);

            await FadeTransition.FadeIn(_roundText, .1f);

            return;
        }

        // 通常被弾時
        _roundText.text = "Damaged!";
        NetworkManager.Emit(roundUpdateReq);

        await FadeTransition.FadeIn(_roundText, .1f);
        await UniTask.Delay(TimeSpan.FromSeconds(1), true);
        await FadeTransition.FadeOut(SystemProperty.FadeCanvasGroup, .5f);

        _roundText.text = "";
        Time.timeScale  = 1;

        // 生成アイテムをリセット
        ItemManager.ClearGeneratedItem();

        // 各プレイヤーを所定位置に戻す
        MainGameController.Instance.ResetPlayersPosition();

        // 描画中かもしれない射線を開放
        ShotLineUtil.FreeLineData(ShotLineDrawer.DrawingData);

        Projectile.DestroyAllBullets();

        _roundText.text = $"Round {RoundManager.CurrentRound.ToString()}";

        await FadeTransition.FadeIn(SystemProperty.FadeCanvasGroup, .5f);

        RoundManager.RoundMove = false;

        // 相手のラウンド進行が済んでから操作可能にさせるため、ここではそれをせず待機
        _statusText.text = "待機中…";
    }

    private void OnPositionChanged(Vector3 pos)
    {
        var playerMoveReq = new PlayerMoveRequest(pos, transform.rotation);

        NetworkManager.Emit(playerMoveReq);
    }
}
