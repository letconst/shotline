using UnityEngine;

public class BigBullet : ActiveItem
{
    //BigBulletが有効の時用のbool、使ぁE��たすとfalse
    public static bool BBOn = false;
    public static bool BBOff = false;

    public static bool ClickBB = false;

    [SerializeField, Header("�r�b�N�o���b�g�̍ő��")]
    private float maxNumBigBullet = 3;

    //最初に実行される
    protected override void Init()
    {
        base.Init();

        //BBOn�����
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
        // 射線が描いてあるときのみ発射
        if (Projectile.currentLineData is {IsFixed: false} && Projectile.currentLineData.Renderer.enabled)
        {
            ItemManager.ShotBtn.GetComponentInChildren<Projectile>().Fire();
            SoundManager.Instance.PlaySE(SELabel.Shot);
        }

        if (BBOn && Projectile.One)
        {
            NumQuantity.CulNum(maxNumBigBullet);

            ClickBB = true;

            LineGaugeController.Clicked();

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
