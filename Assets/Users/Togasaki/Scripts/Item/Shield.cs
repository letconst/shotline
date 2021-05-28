using UnityEngine;

public class Shield : ActiveItem
{

    //Shieldが有効の時用のbool、使い果たすとfalse
    public static bool ShiOn = false;

    GameObject ShieldObj;

    GameObject Player;

    //OriginShieldLocationを定義
    [SerializeField] private Transform OriginShieldLocation;



    //最初に実行される
    protected override void Init()
    {
        //とったらはじめにされる処理
        base.Init();

        ShiOn = true;

        gameObject.transform.position = OriginShieldLocation.position;


    }

    //最後に実行される
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
