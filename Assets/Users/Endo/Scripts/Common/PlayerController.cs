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

        var data = new SendData(EventType.RoundUpdate)
        {
            Self = new PlayerData()
        };

        // 残機ゼロ時
        if (RoundManager.CurrentPlayerLife == 0)
        {
            data.Self.isLose = true;
            _roundText.text  = "Lose!";
            NetworkManager.Emit(data);

            await FadeTransition.FadeIn(_roundText, .1f);

            return;
        }

        // 通常被弾時
        _roundText.text = "Damaged!";
        NetworkManager.Emit(data);

        await FadeTransition.FadeIn(_roundText, .1f);
        await UniTask.Delay(TimeSpan.FromSeconds(.5f), true);
        await FadeTransition.FadeOut(SystemProperty.FadeCanvasGroup, .5f);

        _roundText.text = "";
        Time.timeScale  = 1;

        // 各プレイヤーを所定位置に戻す
        // TODO: 位置はランダムにするため、本来はサーバーで計算
        gameObject.SetActive(false);
        _rivalObject.SetActive(false);

        if (NetworkManager.IsOwner)
        {
            gameObject.transform.position = MainGameProperty.Instance.startPos1P.position;
            _rivalObject.transform.position  = MainGameProperty.Instance.startPos2P.position;
        }
        else
        {
            gameObject.transform.position = MainGameProperty.Instance.startPos2P.position;
            _rivalObject.transform.position  = MainGameProperty.Instance.startPos1P.position;
        }

        gameObject.SetActive(true);
        _rivalObject.SetActive(true);

        // 描画中かもしれない射線を開放
        ShotLineUtil.FreeLineData(ShotLineDrawer.DrawingData);

        _roundText.text = $"Round {RoundManager.CurrentRound.ToString()}";

        await FadeTransition.FadeIn(SystemProperty.FadeCanvasGroup, .5f);

        RoundManager.RoundMove = false;

        // 相手のラウンド進行が済んでから操作可能にさせるため、ここではそれをせず待機
        _statusText.text = "待機中…";
    }

    private void OnPositionChanged(Vector3 pos)
    {
        var data = new SendData(EventType.PlayerMove)
        {
            Self = new PlayerData
            {
                Position = pos,
                Rotation = transform.rotation
            }
        };

        NetworkManager.Emit(data);
    }

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
}
