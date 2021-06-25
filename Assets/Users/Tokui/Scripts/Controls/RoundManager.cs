using UnityEngine;
using UnityEngine.UI;

public class RoundManager : MonoBehaviour
{
    [SerializeField]
    public float PlayerLife = 3;

    [SerializeField]
    private GameObject wallTop;

    [SerializeField]
    private GameObject wallRight;

    [SerializeField]
    private GameObject wallLeft;

    [SerializeField]
    private GameObject wallBottom;

    [SerializeField]
    private float WallSpeedReset;

    [SerializeField]
    private float WallMoveTime;

    [SerializeField]
    private float CountReset;

    [SerializeField]
    private GameObject StartText;

    private bool SuddenDeathFlag;

    private float CountDown;

    private float WallSpeed;

    //サドンデス設定時間
    public float SuddenDeath = 360f;

    // 現在ラウンドが切り替わっている最中かどうか判別
    public static bool RoundMove = false;

    // Start is called before the first frame update
    void Start()
    {
        CountDown = CountReset;
        WallSpeed = WallSpeedReset;
        SuddenDeathFlag = true;
        // テキストを非アクティブにする
        StartText.SetActive(false);

        // ラウンド開始時、壁を非アクティブにする
        wallTop.SetActive(false);
        wallLeft.SetActive(false);
        wallRight.SetActive(false);
        wallBottom.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // カウントダウン
        CountDown -= Time.deltaTime;

        if (CountDown <= 0)
        {
            if (SuddenDeathFlag == true)
            {
                // サドンデス開始時、壁をアクティブにする
                wallTop.SetActive(true);
                wallLeft.SetActive(true);
                wallRight.SetActive(true);
                wallBottom.SetActive(true);

                // 壁(上)をdeltatime分中央に向かって移動
                Transform wallTransForm = wallTop.transform;
                Vector3 wallTopPos = wallTransForm.position;
                wallTopPos.z -= WallSpeed * Time.deltaTime;
                wallTransForm.position = wallTopPos;

                // 壁(左)をdeltatime分中央に向かって移動
                Transform wallLeftTransForm = wallLeft.transform;
                Vector3 wallLeftPos = wallLeftTransForm.position;
                wallLeftPos.x += WallSpeed * Time.deltaTime;
                wallLeftTransForm.position = wallLeftPos;

                // 壁(右)をdeltatime分中央に向かって移動
                Transform wallRightTransForm = wallRight.transform;
                Vector3 wallRightPos = wallRightTransForm.position;
                wallRightPos.x -= WallSpeed * Time.deltaTime;
                wallRightTransForm.position = wallRightPos;

                // 壁(下)をdeltatime分中央に向かって移動
                Transform wallBottomTransForm = wallBottom.transform;
                Vector3 wallBottomPos = wallBottomTransForm.position;
                wallBottomPos.z += WallSpeed * Time.deltaTime;
                wallBottomTransForm.position = wallBottomPos;

                // テキストをアクティブにする
                StartText.SetActive(true);

                if (CountDown <= -WallMoveTime)
                {
                    // WallSpeedをリセット
                    WallSpeed = WallSpeedReset;

                    // カウントをリセット
                    CountDown = CountReset;

                    // テキストを非アクティブにする
                    StartText.SetActive(false);

                }
            }
        }

        HitVerification();
    }

    public void HitVerification()
    {
        // 弾が当たったら操作不能にし、ライフを1減らす
        if (RoundMove == true)
        {
            // ラウンドが進んだらフラグをfalseにする
            SuddenDeathFlag = false;

            // ラウンドが進んだら壁を非アクティブにする
            wallTop.SetActive(false);
            wallLeft.SetActive(false);
            wallRight.SetActive(false);
            wallBottom.SetActive(false);

            // プレイヤーのライフを1減らす
            PlayerLife--;

            if (PlayerLife >0)
            {
                // ラウンド切り替え
                RoundMove = false;
            }
            // プレイヤーのライフが0になったらリザルトへ
            if (PlayerLife == 0)
            {
                //リザルトへ
                Debug.Log("Result");
            }
        }
    }
}