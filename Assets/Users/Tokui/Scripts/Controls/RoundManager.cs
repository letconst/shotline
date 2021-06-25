using UnityEngine;

public class RoundManager : MonoBehaviour
{
    [SerializeField]
    public float PlayerLife = 3; //プレイヤーのライフ

    // 現在ラウンドが切り替わっている最中かどうか判別
    public static bool RoundMove = false;

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