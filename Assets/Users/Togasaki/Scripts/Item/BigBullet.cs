using UnityEngine;

public class BigBullet : PassiveItem
{
    //BigBulletが有効の時用のbool、使い果たすとfalse
    public static bool BBOn = false;
    public static bool BBOff = false;

    //最初に実行される
    protected override void Init()
    {
        base.Init();

        //BBOnをつける
        BBOff = false;
        BBOn = true;

    }

    //最後に実行される
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
