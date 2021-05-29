using UnityEngine;

public class BigBullet : PassiveItem
{
    //BigBulletが有効の時用のbool、使い果たすとfalse
    public static bool BBOn = false;

    //最初に実行される
    protected override void Init()
    {
        base.Init();

        //BBOnをつける
        BBOn = true;

        //残り回数
        Projectile.BBnum = 4;

        //スピードを変える
        Projectile.Speed *= 0.8f;

    }

    //最後に実行される
    protected override void Terminate()
    {
     
        //スピードを戻す
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
