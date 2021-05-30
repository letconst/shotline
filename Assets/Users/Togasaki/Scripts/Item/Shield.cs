using UnityEngine;

public class Shield : ActiveItem
{


    [SerializeField] private GameObject ShieldPrefab;

    GameObject Player;


    //�ŏ��Ɏ��s�����
    protected override void Init()
    {
        //�Ƃ�����͂��߂ɂ���鏈��
        base.Init();


    }

    //�Ō�Ɏ��s�����
    protected override void Terminate()
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


        GameObject ShieldObj = Instantiate(ShieldPrefab, pos + vec * 2, Player.transform.rotation);

        Terminate();

    }

    protected override void UpdateFunction()
    {

    }

}
