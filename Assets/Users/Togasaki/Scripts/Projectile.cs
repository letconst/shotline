using UnityEngine;

public class Projectile : MonoBehaviour
{
    /*
     
    "Projectile"クラスの概要
     
    もしボタンが押されたらShotLineDrawerクラスの座標を取得し、その座標をなぞるように弾丸を発射する。
    変数"Speed"に弾丸の速さを指定できる。
     
     */

    //弾丸の普段いる場所（マップ外）
    [SerializeField] private Transform OriginBulletLocation;

    //弾丸のprefab
    public GameObject Bullet;
    public static Vector3 OriginBulletScale;

    //はやさ
    public static float Speed = 10;
    public static float OriginSpeed = 10;

    //射線の座標をいれる配列
    Vector3[] FingerPositions;

    //射線の変数
    LineRenderer Line;

    ////Updateで使う用のint
    int i;

    //ボタンが押された用のflag
    bool flag;

    //一回だけ射線の座標を取得
    bool One = true;

    //BB用のint
    public static int BBnum = 2;


    private void Start()
    {
        OriginBulletScale = Bullet.transform.localScale;
    }

    void Update()
    {
        LineAppear();

    }


    //射線に沿って弾丸を移動させる処理
    private void LineAppear()
    {
        //もし射線が空だったら
        if (Line == null)
        {
            Line = GameObject.FindGameObjectWithTag("ShotLine").GetComponent<LineRenderer>();
        }


        //ラインが引かれていたら
        if (Line != null && Line.enabled && One)
        {
            //配列に射線の全座標をいれる
            FingerPositions = ShotLineDrawer.GetFingerPositions();
            One = false;
        }

        //もしラインがあってボタンが押されたら
        if (Line != null && Line.enabled && flag)
        {

            //弾を実際に動かす部分
            if (i == FingerPositions.Length - 1)
            {
                Bullet.transform.position = Vector3.MoveTowards(FingerPositions[FingerPositions.Length - 2], FingerPositions[FingerPositions.Length - 1], Speed * Time.deltaTime);
            }
            else
            {
                if (i == 0)
                {
                    Bullet.transform.position = FingerPositions[0];
                }
                else
                {
                    Bullet.transform.position = Vector3.MoveTowards(Bullet.transform.position, FingerPositions[i + 1], Speed * Time.deltaTime);
                }
            }

            if (Bullet.transform.position == FingerPositions[i + 1])
            {
                i++;
            }

            //弾が動き終わったら
            if (i == FingerPositions.Length - 1)
            {
                ShotLineDrawer.ClearLine();
                i = 0;
                Bullet.transform.position = OriginBulletLocation.position;
                flag = false;
            }

        }


    }


    //射撃ボタン、flagをtrueに
    public void Fire()
    {
        //一回だけ座標を取得用
        One = true;

        //BigBullet用
        if (Line != null && Line.enabled && BigBullet.BBOn && flag == false)
        {
            BBnum--;

            if (BBnum < 0)
            {
                BigBullet.BBOn = false;
            }
        }

        //球が動いているとき用
        flag = true;

    }
}
