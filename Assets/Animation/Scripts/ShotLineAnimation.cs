using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotLineAnimation : MonoBehaviour
{
    //Animator�R���|�[�l���g
    private Animator animator;

    void Start()
    {
        //�L�����N�^�[�̃q�G�����L�[�ɑ��݂���A�j���[�^�[���Q��
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
