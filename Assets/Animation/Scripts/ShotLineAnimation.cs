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
            animator.SetBool("Walk", true);
        }
        else
        {
            animator.SetBool("Walk", false);
            
        }
    }
}
