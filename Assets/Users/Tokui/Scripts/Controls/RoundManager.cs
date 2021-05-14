using UnityEngine;

public class RoundManager : MonoBehaviour
{
    [SerializeField]
    public Object Player;

    [SerializeField]
    public float PlayerLife = 3;

    // 現在ラウンドが切り替わっている最中かどうか判別
    public bool RoundMove = false;
    // ラウンドが切り替わっている最中は操作が出来なくなるようにする
    public static bool MoveStop = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // ラウンドが切り替わっている最中でなければ処理を行う
        if (RoundMove == false)
        {
            MoveStop = false;
        }
    }
}
