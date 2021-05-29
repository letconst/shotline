using UnityEngine;

public class Shield : ActiveItem
{

    //Shield���L���̎��p��bool�A�g���ʂ�����false
    public static bool ShiOn = false;

    GameObject ShieldObj;

    GameObject Player;

    //OriginShieldLocation���`
    [SerializeField] private Transform OriginShieldLocation;



    //�ŏ��Ɏ��s�����
    protected override void Init()
    {
        //�Ƃ�����͂��߂ɂ���鏈��
        base.Init();

        ShiOn = true;
    }

    //�Ō�Ɏ��s�����
    protected override void Terminate()
    {
        ShieldObj = GameObject.FindGameObjectWithTag("Shield");

        ShieldObj.transform.position = OriginShieldLocation.position;

        base.Terminate();

    }


    protected override void OnClickButton()
    {
        if (ShiOn)
        {

            Player = GameObject.FindGameObjectWithTag("Player");
            Vector3 vec;
            vec = Player.transform.forward;
            vec.z += 2f;

            Vector3 pos;
            pos = Player.transform.position;

            ShieldObj = GameObject.FindGameObjectWithTag("Shield");

            ShieldObj.transform.position = pos + vec;
        }
    }

    protected override void UpdateFunction()
    {
        if (ShiOn == false)
        {
            Terminate();
        }

    }

}
