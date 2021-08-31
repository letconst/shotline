using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallJudge : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        // Wallタグを判別
        // 個数が1個なら return をする
        // collision.contacts から中身を見ていき、tan で角度を求め、180度(±5度)の範囲から判定を確認する
        // 接触点が3点以上ある時に角度の線の全通りの比較をできるようにする
        // 比較していく途中でプレイヤーと接触する線が見つかったら死亡判定をとり、その時点で処理を終了する

        // 配列なら collision.contacts.length で個数を確認できる
        // リストなら collision.contacts.count で個数を確認できる

        if (collision.gameObject.tag == "Wall")
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                if (collision.contacts.Length == 1)
                {
                    Debug.Log("aa");
                    return;
                }

                else if(collision.contacts.Length == 2)
                {
                    Debug.Log("Death");
                }
            }
        }    
    }
}
