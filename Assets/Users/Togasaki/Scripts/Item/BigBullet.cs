using UnityEngine;

public class BigBullet : ItemBase
{
    //�Q�[������Bullet���擾
    GameObject Bullet;
    Projectile GetProjectile;

    //BigBullet���L���̎��p��bool�A�g���ʂ�����false
    public static bool BBOn = false;

    [SerializeField] private Transform OriginBulletLocation;



    //�ŏ��Ɏ��s�����
    protected override void Init()
    {
        //����
        base.Init();

        //BBOn������
        BBOn = true;

        //�c���
        Projectile.BBnum = 2;

        //OriginBullet���������Ă��̒���Projectile��ϐ���
        Bullet = GameObject.Find("OriginBullet");
        GetProjectile = Bullet.GetComponent<Projectile>();

        //�e�̃X�P�[����ς���
        Bullet.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

        //�X�s�[�h��ς���
        Projectile.Speed *= 0.8f;

        gameObject.transform.position = OriginBulletLocation.position;


    }

    //�Ō�Ɏ��s�����
    protected override void Terminate()
    {
        base.Terminate();

        //�e�̑傫����߂�
        Bullet.transform.localScale = Projectile.OriginBulletScale;
        
        //�X�s�[�h��߂�
        Projectile.Speed = Projectile.OriginSpeed;

    }


    private void OnTriggerEnter(Collider other)
    {
        Init();
    }

    protected override void UpdateFunction()
    {
        if (BBOn == false)
        {
            Terminate();
        }

    }
}
