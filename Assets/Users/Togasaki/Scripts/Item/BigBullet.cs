using UnityEngine;

public class BigBullet : ActiveItem
{
    //BigBullet縺梧怏蜉ｹ縺ｮ譎ら畑縺ｮbool縲∽ｽｿ縺・棡縺溘☆縺ｨfalse
    public static bool BBOn = false;
    public static bool BBOff = false;

    public static bool OneBB = true;

    public static bool ClickBB = false;

    [SerializeField, Header("ビックバレットの最大量")]
    private float maxNumBigBullet = 3;

    //譛蛻昴↓螳溯｡後＆繧後ｋ
    protected override void Init()
    {
        base.Init();

        //BBOnをつける
        BBOff = false;
        BBOn = true;
        OneBB = true;
        ClickBB = false;

    }

    //譛蠕後↓螳溯｡後＆繧後ｋ
    public override void Terminate()
    {
        BBOff = false;
        BBOn = false;
        base.ClearItemIcon();

        base.Terminate();

    }

    protected override void OnClickButton()
    {
        if (BBOn && OneBB)
        {
            NumQuantity.CulNum(maxNumBigBullet);

            ClickBB = true;

            Projectile.ScaleRatio = 1.5f;

            OneBB = false;

            if (ItemManager.currentNum == maxNumBigBullet)
            {
                OneBB = true;
                BBOff = true;
                Terminate();

            }

            ItemManager.ShotBtn.GetComponentInChildren<Projectile>().Fire();
        }

    }

    protected override void UpdateFunction()
    {

    }
}
