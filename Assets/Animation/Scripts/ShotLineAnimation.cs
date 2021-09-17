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
        //�ˌ��A�j���[�V����
        if (Projectile.One)
        {
            animator.SetTrigger("Shot");
        }

        //���s�A�j���[�V����
        if(CharaMove.IsMoving)
        {
            animator.SetBool("Walk", true);
        }
        else
        {
            animator.SetBool("Walk", false);
        }

        //�X���X�^�[�A�j���[�V����
        if (Thruster.ClickButton)
        {
            animator.SetTrigger("Dash");
        }

    }
}
