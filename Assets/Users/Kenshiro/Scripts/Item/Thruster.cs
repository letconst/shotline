using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thruster : ActiveItem
{
    GameObject Player;
    CharaMove Chara;

    CharaMove BoostSpeed;

    //�ړ����x�J�E���g�_�E���p�ϐ�
    public float MoveCountDown;
    bool ClickButton;

    int seconds;

    //���G���ԃJ�E���g�_�E���p�ϐ�
    public float FlagCountDown;

    protected override void Init()
    {
        base.Init();

        //�X���X�^�[�̏����m�F
        Player = GameObject.FindGameObjectWithTag("Player");
        Chara = Player.GetComponent<CharaMove>();

    }

    protected override void OnClickButton()
    {
        //�{�^�����������Ƃ��ړ����x1.5�{
        Chara.speedRatio = 1.5f;

        //�{�^������������e�̃t���O������ true �ɂ���
        Chara.Muteki = true;

        //�{�^�����������Ƃ��J�E���g�J�n
        ClickButton = true;

        ClearItemIcon();
    }

    protected override void UpdateFunction()
    {

        //�ړ����x1.5�{�J�E���g
        if (ClickButton && MoveCountDown > 0)
        {
            MoveCountDown -= Time.deltaTime;
            seconds = (int)MoveCountDown;
        }

        //���G���Ԃ̃J�E���g
        if (Chara.Muteki == true)
        {
            FlagCountDown -= Time.deltaTime;
            seconds = (int)MoveCountDown;

            //true �̎��A�����Ă�������Ɉړ�
            BoostSpeed = Player.GetComponent<CharaMove>();

            BoostSpeed.moveX = BoostSpeed.CurrentSpeed;
            BoostSpeed.moveZ = BoostSpeed.CurrentSpeed;
        }

        //�ړ����x�����ɖ߂�
        if (MoveCountDown <= 0)
        {
            Chara.speedRatio = 1f;
        }

        //���G����
        //�t���O�� false �ɖ߂�
        if (FlagCountDown <= 0)
        {
            Chara.Muteki = false;
        }

        //���J�E���g��0�ɂȂ����� Terminate ���Ă�
        if (MoveCountDown <= 0 && FlagCountDown <= 0)
        {
            Terminate();
        }
    }

    public override void Terminate()
    {
        base.Terminate();
    }
}
