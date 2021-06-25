using UnityEngine;

public class BigBullet : ActiveItem
{
    //BigBulletが有効の時用のbool、使い果たすとfalse
    public static bool BBOn = false;
    public static bool BBOff = false;

    public static bool OneBB = true;

    public static bool ClickBB = false;

    [SerializeField, Header("ビックバレットの最大量")]
    private float maxNumBigBullet = 3;


    public GameObject stBtn;

    //最初に実行される
    protected override void Init()
    {
        base.Init();

        stBtn = GameObject.Find("Shot");

        //BBOnをつける
        BBOff = false;
        BBOn = true;
        OneBB = true;
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

        NumQuantity.CulNum(maxNumBigBullet);

        if (BBOn && Projectile.Line != null && Projectile.Line.enabled && OneBB)
        {
            ClickBB = true;

            Projectile.ScaleRatio = 1.5f;

            OneBB = false;

            if (ItemManager.currentNum == maxNumBigBullet)
            {
                OneBB = true;
                BBOff = true;
                Terminate();

            }

            stBtn.GetComponent<Projectile>().Fire();
        }

    }

    protected override void UpdateFunction()
    {

    }
}
