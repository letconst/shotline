using UnityEngine;

public class BigBullet : ActiveItem
{
    //BigBulletが有効の時用のbool、使い果たすとfalse
    public static bool BBOn = false;
    public static bool BBOff = false;

    public static bool ClickBB = false;

    [SerializeField, Header("ビックバレットの最大量")]
    private float maxNumBigBullet = 3;

    //最初に実行される
    protected override void Init()
    {
        base.Init();

        //BBOnをつける
        BBOff = false;
        BBOn = true;
        ClickBB = false;

    }

    //最後に実行される
    public override void Terminate()
    {
        BBOff = false;
        BBOn = false;
        base.ClearItemIcon();

        base.Terminate();

    }

    protected override void OnClickButton()
    {
        ItemManager.ShotBtn.GetComponentInChildren<Projectile>().Fire();

        if (BBOn && Projectile.Line != null && Projectile.Line.enabled&&Projectile.One)
        {
            NumQuantity.CulNum(maxNumBigBullet);

            ClickBB = true;

            Projectile.ScaleRatio = 1.5f;


            if (ItemManager.currentNum == maxNumBigBullet)
            {
                BBOff = true;
                Terminate();

            }

        }

    }

    protected override void UpdateFunction()
    {

    }
}
