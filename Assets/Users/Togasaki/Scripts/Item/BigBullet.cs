using UnityEngine;

public class BigBullet : PassiveItem
{
    //BigBullet���L���̎��p��bool�A�g���ʂ�����false
    public static bool BBOn = false;

    //�ŏ��Ɏ��s�����
    protected override void Init()
    {
        base.Init();

        //BBOn������
        BBOn = true;

        //�c���
        Projectile.BBnum = 4;

        //�X�s�[�h��ς���
        Projectile.Speed *= 0.8f;

    }

    //�Ō�Ɏ��s�����
    protected override void Terminate()
    {
     
        //�X�s�[�h��߂�
        Projectile.Speed = Projectile.OriginSpeed;

        base.Terminate();

    }


    protected override void UpdateFunction()
    {
        if (BBOn == false)
        {
            Terminate();
        }

    }
}
