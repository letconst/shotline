using UnityEngine;

public class Shield : ItemBase
{

    //Shieldが有効の時用のbool、使い果たすとfalse
    public static bool ShiOn = false;

    //OriginShieldLocationを定義
    [SerializeField] private Transform OriginShieldLocation;


    //最初に実行される
    public override void Init()
    {
        //いる
        base.Init();

        ShiOn = true;

        gameObject.transform.position = OriginShieldLocation.position;


    }

    //最後に実行される
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
