using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallJudge : MonoBehaviour
{
    

    void Start()
    {

    }

    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        //外側の壁に触れたら死亡判定を取り次のラウンドへ

        if (!collision.collider.CompareTag("SotoWall")) return;

        Debug.Log("DEATH");
    }


}

