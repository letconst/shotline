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

        //BBOn������
        BBOff = false;
        BBOn = true;

    }

    //�Ō�Ɏ��s�����
    public override void Terminate()
    {

        BBOn = false;

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
