using UnityEngine;

public class Shield : ActiveItem
{
    [SerializeField, Header("シールドの最大量")]
    private int maxNumShield = 1;


    [SerializeField] private GameObject ShieldPrefab;

    GameObject Player;

    //最初に実行される
    protected override void Init()
    {
        //とったらはじめにされる処理
        base.Init();

    }

    //最後に実行される
    public override void Terminate()
    {
        ClearItemIcon();
        base.Terminate();

    }


    protected override void OnClickButton()
    {
        base.OnClickButton();

        NumQuantity.CulNum(maxNumShield);

        Player = GameObject.FindGameObjectWithTag("Player");
        Vector3 vec;
        vec = Player.transform.forward;

        Vector3 pos;
        pos = Player.transform.position;

        if (NetworkManager.IsConnected)
        {
            NetworkManager.Instantiate("Prefabs/Models/Shield", pos + vec * 1, Player.transform.rotation);
        }
        else
        {
            GameObject ShieldObj = Instantiate(ShieldPrefab, pos + vec * 1, Player.transform.rotation);
        }

        // if (ItemManager.currentShieldCount == maxNumShield)
        // {
        //     Terminate();
        // }

        Terminate();
    }

    protected override void UpdateFunction()
    {

    }

}
