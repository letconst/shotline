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
        //ŠO‘¤‚Ì•Ç‚ÉG‚ê‚½‚ç€–S”»’è‚ğæ‚èŸ‚Ìƒ‰ƒEƒ“ƒh‚Ö

        if (!collision.collider.CompareTag("SotoWall")) return;

        Debug.Log("DEATH");
    }


}

