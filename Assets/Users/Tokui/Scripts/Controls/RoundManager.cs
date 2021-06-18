using UnityEngine;

public class RoundManager : MonoBehaviour
{
    [SerializeField]
    public float PlayerLife = 3;

    //サドンデス設定時間
    public float SuddenDeath = 360f;

    //壁の移動速度
    public float WallSpeed = 1f;

    // 現在ラウンドが切り替わっている最中かどうか判別
    public static bool RoundMove = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HitVerification();
    }

    public void HitVerification()
    {
        // 弾が当たったら操作不能にし、ライフを1減らす
        if (RoundMove == true)
        {
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