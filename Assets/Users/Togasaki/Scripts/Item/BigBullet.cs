using UnityEngine;

public class BigBullet : PassiveItem
{
    //BigBullet���L���̎��p��bool�A�g���ʂ�����false
    public static bool BBOn = false;
    public static bool BBOff = false;



    //�ŏ��Ɏ��s�����
    protected override void Init()
    {
        base.Init();

        Projectile.BBnum = 3;

        //BBOn������
        BBOff = false;
        BBOn = true;
        Projectile.ScaleRatio = 1.5f;

    }

    //�Ō�Ɏ��s�����
    public override void Terminate()
    {
        BBOff = false;
        BBOn = false;
        Projectile.ScaleRatio = 1f;

        base.ClearItemIcon();

        base.Terminate();

    }

    protected override void UpdateFunction()
    {
        if (BBOff)
        {
            Terminate();
        }

    }
}
