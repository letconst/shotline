using UnityEngine;

public class BigBullet : ActiveItem
{
    //BigBullet���L���̎��p��bool�A�g���ʂ�����false
    public static bool BBOn = false;
    public static bool BBOff = false;

    public static bool OneBB = true;

    public static bool ClickBB = false;

    [SerializeField, Header("�r�b�N�o���b�g�̍ő��")]
    private float maxNumBigBullet = 3;


    public GameObject stBtn;

    //�ŏ��Ɏ��s�����
    protected override void Init()
    {
        base.Init();

        stBtn = GameObject.Find("Shot");

        //BBOn������
        BBOff = false;
        BBOn = true;
        OneBB = true;
        ClickBB = false;

    }

    //�Ō�Ɏ��s�����
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
