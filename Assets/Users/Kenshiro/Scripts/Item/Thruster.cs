using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thruster : ActiveItem
{
    GameObject Player;
    CharaMove moveSpeed;

    protected override void Init()
    {
        //�X���X�^�[�̏����m�F
     
        base.Init();
    }

    protected override void OnClickButton()
    {

        //�{�^�����������Ƃ��ړ����x1.5�{
        Player = GameObject.Find("Player");
        moveSpeed = Player.GetComponent<CharaMove>();
        moveSpeed.speed *= 1.5f;


        //�{�^�����������Ƃ����G���Ԃ̃J�E���g�̊J�n
    }

    protected override void UpdateFunction()
    {

        //�ړ����x1.5�{�J�E���g


        //���G���Ԃ̃J�E���g


        //���G���Ԃ��߂����� Terminate ���Ă�


    }

    protected override void Terminate()
    {
        //�ړ����x�����ɖ߂�



        //���G����


        base.Terminate();
    }
}
