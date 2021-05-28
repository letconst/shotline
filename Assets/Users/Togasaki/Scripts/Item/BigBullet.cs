using UnityEngine;

public class BigBullet : ItemBase
{
    //ゲーム内のBulletを取得
    GameObject Bullet;
    Projectile GetProjectile;

    //BigBulletが有効の時用のbool、使い果たすとfalse
    public static bool BBOn = false;

    [SerializeField] private Transform OriginBulletLocation;



    //最初に実行される
    protected override void Init()
    {
        //いる
        base.Init();

        //BBOnをつける
        BBOn = true;

        //残り回数
        Projectile.BBnum = 2;

        //OriginBulletをさがしてその中のProjectileを変数化
        Bullet = GameObject.Find("OriginBullet");
        GetProjectile = Bullet.GetComponent<Projectile>();

        //弾のスケールを変える
        Bullet.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

        //スピードを変える
        Projectile.Speed *= 0.8f;

        gameObject.transform.position = OriginBulletLocation.position;


    }

    //最後に実行される
    protected override void Terminate()
    {
        base.Terminate();

        //弾の大きさを戻す
        Bullet.transform.localScale = Projectile.OriginBulletScale;
        
        //スピードを戻す
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
