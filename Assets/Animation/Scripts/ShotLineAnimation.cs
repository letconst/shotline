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
            animator.Play("Player@Firing Rifle", 0, 0);
        }

        if(CharaMove.IsMoving)
        {
            animator.Play("Player@Walk Forward", 0, 0);
        }

    }
}
