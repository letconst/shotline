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
        //�O���̕ǂɐG�ꂽ�玀�S�������莟�̃��E���h��

        if (!collision.collider.CompareTag("SotoWall")) return;

        Debug.Log("DEATH");
    }


}

