using UnityEngine;

public class Shield : ItemBase
{

    //Shield���L���̎��p��bool�A�g���ʂ�����false
    public static bool ShiOn = false;

    //OriginShieldLocation���`
    [SerializeField] private Transform OriginShieldLocation;


    //�ŏ��Ɏ��s�����
    public override void Init()
    {
        //����
        base.Init();

        ShiOn = true;

        gameObject.transform.position = OriginShieldLocation.position;


    }

    //�Ō�Ɏ��s�����
    protected override void Terminate()
    {
        base.Terminate();

        GameObject Shield;
        Shield = GameObject.Find("OriginShield");

        Shield.transform.position = OriginShieldLocation.position;

    }


    private void OnTriggerEnter(Collider other)
    {
        Init();
    }

    protected override void UpdateFunction()
    {
        if (ShiOn == false)
        {
            Terminate();
        }

    }
}
