using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHit : MonoBehaviour
{


    //当たった時のエフェクトを保存しておく変数
    public GameObject effect;


    private void OnCollisionEnter(Collision collision)
    {
        // タグチェック
        if(collision.gameObject.tag == "Player")
        {
            //エフェクト作成
            Instantiate(effect, this.transform.position, Quaternion.identity);
            //アイテムは削除
            Destroy(this.gameObject);
        }
    }
}
