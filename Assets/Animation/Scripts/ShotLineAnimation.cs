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
        //射撃アニメーション
        if (Projectile.One)
        {
            animator.SetTrigger("Shot");
        }

        //歩行アニメーション
        if(CharaMove.IsMoving)
        {
            animator.SetBool("Walk", true);
        }
        else
        {
            animator.SetBool("Walk", false);
        }

        //スラスターアニメーション
        if (Thruster.ClickButton)
        {
            animator.SetTrigger("Dash");
        }

    }
}
