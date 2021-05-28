using UnityEngine;
using System.Collections.Generic;



public class Projectile : MonoBehaviour
{
    /*
     
    "Projectile"クラスの概要
     
    もしボタンが押されたらShotLineDrawerクラスの座標を取得し、その座標をなぞるように弾丸を発射する。
    変数"Speed"に弾丸の速さを指定できる。
     
     */

    public struct BulletInfo
    {
        //弾丸のprefab
        public GameObject Bullet;

        //射線の座標をいれる配列
        public Vector3[] FP;

        public BulletInfo(GameObject bullet, Vector3[] fp)
        {
            Bullet = bullet;
            FP = fp;
        }
    }

    [SerializeField] private GameObject BulletPrefab;

    //リストに弾の情報を
    //最終地点に到達したらリストから消す
    private List<BulletInfo> BulletList = new List<BulletInfo>();

    //弾丸の普段いる場所（マップ外）
    [SerializeField] private Transform OriginBulletLocation;

    public static Vector3 OriginBulletScale;

    //はやさ
    public static float Speed = 10;
    public static float OriginSpeed = 10;

    //射線の変数
    LineRenderer Line;

    ////Updateで使う用のint
    int index;

    //ボタンが押された用のflag
    bool flag;

    //一回だけ射線の座標を取得
    bool One = false;

    //BB用のint
    public static int BBnum = 2;


    //private void Start()
    //{

    //    OriginBulletScale = Bullet.transform.localScale;
    //}

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
        if (Line != null && Line.enabled && One )
        {

            Vector3[] FingerPositions = ShotLineDrawer.GetFingerPositions();

            GameObject BI = Instantiate(BulletPrefab, FingerPositions[0], Quaternion.identity);

            //配列に射線の全座標をいれる
            BulletList.Add(new BulletInfo(BI, ShotLineDrawer.GetFingerPositions()));

            One = false;

        }

        //もしラインがあってボタンが押されたら
        if (BulletList.Count>0)
        {
            for (int i = 0; i < BulletList.Count; i++)
            {

                Debug.Log(BulletList[i].FP.Length);

                //弾を実際に動かす部分
                //もし射線の長さが最後だったら
                if (index == BulletList[i].FP.Length - 1)
                {
                    BulletList[i].Bullet.transform.position = Vector3.MoveTowards(BulletList[i].FP[BulletList[i].FP.Length-2], BulletList[i].FP[BulletList[i].FP.Length - 1], Speed * Time.deltaTime);
                }
                else
                {
                    //射線の最初
                    if (index == 0)
                    {
                        BulletList[i].Bullet.transform.position = BulletList[i].FP[0];
                    }
                    else
                    {
                        //現在の射線の位置から次の射線の位置まで移動
                        BulletList[i].Bullet.transform.position = Vector3.MoveTowards(BulletList[i].Bullet.transform.position, BulletList[i].FP[index + 1], Speed * Time.deltaTime);
                    }
                }

                //もし弾が次の位置まで到達したら、その次の位置を読み込む
                if (BulletList[i].Bullet.transform.position == BulletList[i].FP[index + 1])
                {
                    index++;
                }

                //弾が動き終わったら、もしくは壁かシールドに当たったら
                if (index == BulletList[i].FP.Length - 1)
                {
                    ShotLineDrawer.ClearLine();
                    i = 0;
                    flag = false;
                    One = false;
                }

            }

        }

    }


    //射撃ボタン、flagをtrueに
    public void Fire()
    {
        //射線の固定
        ShotLineDrawer.FixLine();

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
