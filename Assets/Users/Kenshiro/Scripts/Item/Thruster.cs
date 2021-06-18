using UnityEngine;

public class Thruster : ActiveItem
{
    GameObject Player;
    CharaMove Chara;

    CharacterController BoostSpeed;
    Vector3 Direction;
    //�ړ����x�J�E���g�_�E���p�ϐ�
    public float MoveCountDown;
    bool ClickButton;


    //���G���ԃJ�E���g�_�E���p�ϐ�
    public float FlagCountDown;

    protected override void Init()
    {
        base.Init();

        //�X���X�^�[�̏����m�F
        Player = GameObject.FindGameObjectWithTag("Player");
        Chara = Player.GetComponent<CharaMove>();

        //CharacterController���擾
        BoostSpeed = Player.GetComponent<CharacterController>();

    }

    protected override void OnClickButton()
    {
        //�{�^�����������Ƃ��ړ����x1.5�{
        Chara.speedRatio = 1.5f;

        //�{�^������������e�̃t���O������ true �ɂ���
        Chara.Thruster = true;

        //�{�^�����������Ƃ��J�E���g�J�n
        ClickButton = true;

        //�{�^����������Player�̌����Ă���������擾
        Direction = Player.transform.rotation * new Vector3(0, 0, Chara.CurrentSpeed);

        ClearItemIcon();
    }

    protected override void UpdateFunction()
    {

        //�ړ����x1.5�{�J�E���g
        if (ClickButton && MoveCountDown > 0)
        {
            MoveCountDown -= Time.deltaTime;
        }

        //���G���Ԃ̃J�E���g
        if (Chara.Thruster == true)
        {
            FlagCountDown -= Time.deltaTime;

            //true �̎��A�����Ă�������Ɉړ�
            BoostSpeed.SimpleMove(Direction);
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
            Chara.Thruster = false;
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
