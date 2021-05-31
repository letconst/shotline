using UnityEngine;

public class Shield : ActiveItem
{


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

        base.ClearItemIcon();
        base.Terminate();

    }


    protected override void OnClickButton()
    {

        Player = GameObject.FindGameObjectWithTag("Player");
        Vector3 vec;
        vec = Player.transform.forward;

        Vector3 pos;
        pos = Player.transform.position;


        GameObject ShieldObj = Instantiate(ShieldPrefab, pos + vec * 1, Player.transform.rotation);

        Terminate();

    }

    protected override void UpdateFunction()
    {

    }

}
