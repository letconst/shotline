using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallJudge : MonoBehaviour
{
    float GetAngle(Vector2 start, Vector2 target)
    {
        Vector2 dt = target - start;
        float rad = Mathf.Atan2(dt.y, dt.x);
        float degree = rad * Mathf.Rad2Deg;

        return degree;
    }

    void Start()
    {

    }

    void Update()
    {
        
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!collision.collider.CompareTag("Wall")) return;

        // Wallタグを判別
        // 個数が1個なら return をする
        // collision.contacts から中身を見ていき、tan で角度を求め、180度(±5度)の範囲から判定を確認する
        // 接触点が3点以上ある時に角度の線の全通りの比較をできるようにする
        // 比較していく途中でプレイヤーと接触する線が見つかったら死亡判定をとり、その時点で処理を終了する

        // 配列なら collision.contacts.length で個数を確認できる
        // リストなら collision.contacts.count で個数を確認できる

        // Vector3(三次元)のX、ZのデータをVector2(二次元)のX、Yに変換
        Vector2 pos = new Vector2(transform.position.x, transform.position.z);

        // 配列からStackに変換
        Stack<ContactPoint> s = new Stack<ContactPoint>(collision.contacts);

        Debug.Log(collision.contacts.Length);

        while (s.Count > 0)
        {
            // Stackから一つ取り出して変数に格納
            ContactPoint poppedContact = s.Pop();

            // Stackをforeachで回し、取り出したものと比較していく
            foreach (ContactPoint contact in s)
            {

                // 取り出したcontact(poppedContact)と今、回してるcontactをVector2に変換
                // 各contactの座標とプレイヤーの座標から角度を求める
                // 角度はプレイヤー(transform.position)からcontactの方向で統一して求める
                // 二つの角度を一方から一方を引く
                // 引いた値が180だったらdeath

                Vector2 contact1 = ContactPoint(contact);
                Vector2 popContact = ContactPoint(poppedContact);

                float tan = GetAngle(contact1, popContact);
                Debug.Log(tan);

                if (tan == 180)
                {
                    Debug.Log("DEATH");
                    break;
                }

                else if (tan == -180)
                {
                    Debug.Log("DEATH");
                    break;
                }
            }
        }
    }

    private Vector2 ContactPoint(ContactPoint contact)
    {
        return new Vector2(contact.point.x, contact.point.z);
    }

    //foreach (ContactPoint contact in collision.contacts)
    //{
    //  if (collision.gameObject.tag == "Wall")
    //  {
    //    if (collision.contacts.Length == 2)
    //    {
    //       Debug.Log("Death");
    //    }
    //   }
    //}
}

