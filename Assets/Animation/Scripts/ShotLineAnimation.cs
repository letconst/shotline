using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotLineAnimation : MonoBehaviour
{
    //Animatorコンポーネント
    private Animator animator;

    void Start()
    {
        //キャラクターのヒエラルキーに存在するアニメーターを参照
        this.animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Projectile.One)
        {
            animator.Play("Player@Firing Rifle", 0, 0);
        }

        if(CharaMove.IsMoving)
        {
            animator.Play("Player@Walk Forward", 0, 0);
        }

    }
}
