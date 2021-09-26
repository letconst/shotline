using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private GameObject _rivalObject;
    private Text       _roundText;

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
        SystemUIManager.SetInputBlockerVisibility(true);

        var roundUpdateReq = new RoundUpdateRequest();

        // 残機ゼロ時
        if (RoundManager.CurrentPlayerLife == 0)
        {
            roundUpdateReq.IsLoseRival = true;
            _roundText.text            = "LOSE!";
            SystemUIManager.ShowStatusText(StatusText.TapToTitle);

            MainGameController.isChangeableSceneToTitle = true;
            NetworkManager.Emit(roundUpdateReq);

            await FadeTransition.FadeIn(_roundText, .1f);

            return;
        }

        // 通常被弾時
        _roundText.text = "DAMAGED!";
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
        MainGameProperty.SuddenDeathImg.color = new Color(1, 1, 1, 0);

        // 外壁サイズリセット
        foreach (Transform wall in MainGameProperty.SotoWalls)
        {
            wall.localScale = Vector3.one;
        }

        RoundManager.Instance.SuddenDeathFlag = false;

        _roundText.text = $"ROUND {RoundManager.CurrentRound.ToString()}";

        await FadeTransition.FadeIn(SystemProperty.FadeCanvasGroup, .5f);

        RoundManager.RoundMove = false;

        // 相手のラウンド進行が済んでから操作可能にさせるため、ここではそれをせず待機
        SystemUIManager.ShowStatusText(StatusText.NowWaiting);
    }


    private void OnPositionChanged(Vector3 pos)
    {
        var playerMoveReq = new PlayerMoveRequest(pos, transform.rotation);

        NetworkManager.Emit(playerMoveReq);
    }

    /*
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.tag == "Wall")
        {
            // どちらに当たったかの方向を取得

            // xとzの値を絶対値に変換
            // その後、xとzどちらの値が大きいか判別
            // 大きい方の方向の値をプラスかマイナスで判別

            // X方向の絶対値
            float AbsX = Mathf.Abs(hit.point.normalized.x);
            // Z方向の絶対値
            float AbsZ = Mathf.Abs(hit.point.normalized.z);

            // XとZを比較

            // Xが大きい場合
            bool Xpoint = AbsX > AbsZ;
            // Zが大きい場合
            bool Zpoint = AbsZ > AbsX;

            if (hit.point.normalized.x >= 0.1 & Xpoint)
            {
                Debug.Log("Left");
            }

            else if (hit.point.normalized.x <= -0.1 & Xpoint)
            {
                Debug.Log("Right");
            }

            else if (hit.point.normalized.z >= 0.1 & Zpoint)
            {
                Debug.Log("Botom");
            }

            else if (hit.point.normalized.z <= -0.1 & Zpoint)
            {
                Debug.Log("Top");
            }

            // ↓この情報で斜めの場合は絶対値にする
            // どちらもまったく同じ場合は一旦無視
            //Debug.Log(hit.point.normalized);
        }
    }
    */
}
