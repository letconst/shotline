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

        //射線の現在座標用int
        public int index;


        public BulletInfo(GameObject bullet, Vector3[] fp ,int ind)
        {
            Bullet = bullet;
            FP = fp;
            index = ind;
        }
    }

    [SerializeField] private GameObject BulletPrefab;

    //リストに弾の情報を
    public static List<BulletInfo> BulletList = new List<BulletInfo>();

    public static Vector3 OriginBulletScale;

    //はやさ
    public static float Speed = 10;
    public static float OriginSpeed = 10;

    //射線の変数
    LineRenderer Line;

    //一回だけ射線の座標を取得
    bool One = false;

    //BB用のint
    public static int BBnum = 4;

    //for用
    private int i = 0;

    //射線用
    private bool flag = true;

    void Update()
    {
        LineAppear();

        Debug.Log(flag);

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

            Vector3[] FingerPositions = ShotLineDrawer.GetFingerPositions();

            GameObject BI = Instantiate(BulletPrefab, FingerPositions[0], Quaternion.identity);

            if(BigBullet.BBOn)
            {

                //弾のスケールを変える
                BI.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

                if (BBnum == 0)
                {
                    //弾の大きさを戻す
                    BBnum = 4;
                    BI.transform.localScale = new Vector3(1f, 1f, 1f);
                    BigBullet.BBOn = false;
                }

            }
            //配列に射線の全座標とそれに対応する弾丸をいれる
            BulletList.Add(new BulletInfo(BI, ShotLineDrawer.GetFingerPositions(), 0));

            if(BulletList.Count>1)
            {
                flag = false;
            }

            One = false;

        }


        //もしラインがあってボタンが押されたら
        if (BulletList.Count > 0)
        {

            for (i = 0; i < BulletList.Count; i++)
            {

                //現在の座標を変更できるように変数化
                BulletInfo currentP = BulletList[i];

                //弾を実際に動かす部分

                //もし射線の長さが最後だったら
                if (BulletList[i].index == BulletList[i].FP.Length - 1)
                {
                    BulletList[i].Bullet.transform.position = Vector3.MoveTowards(BulletList[i].FP[BulletList[i].FP.Length - 2], BulletList[i].FP[BulletList[i].FP.Length - 1], Speed * Time.deltaTime);
                }
                else
                {
                    //射線の最初
                    if (BulletList[i].index == 0)
                    {
                        BulletList[i].Bullet.transform.position = BulletList[i].FP[0];
                    }
                    else
                    {
                        //現在の射線の位置から次の射線の位置まで移動
                        BulletList[i].Bullet.transform.position = Vector3.MoveTowards(BulletList[i].Bullet.transform.position, BulletList[i].FP[BulletList[i].index + 1], Speed * Time.deltaTime);
                    }
                }

                //もし弾が次の位置まで到達したら、その次の位置を読み込む
                if (BulletList[i].Bullet.transform.position == BulletList[i].FP[BulletList[i].index + 1])
                {
                    currentP.index++;
                    BulletList[i] = currentP;
                }

                //弾が動き終わったら、もしくは壁かシールドに当たったら
                if (BulletList[i].index == BulletList[i].FP.Length - 1||BulletMovement.BBOn)
                {
                    if (BulletList.Count == 1)
                    {
                        flag = true;
                    }
                    if (flag && BulletList.Count == 1)
                    {
                        ShotLineDrawer.ClearLine();
                    }
                    BulletMovement.BBOn = false;
                    Destroy(BulletList[i].Bullet);
                    BulletList.RemoveAt(i);
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

        if (BigBullet.BBOn && Line != null && Line.enabled)
        {
            BBnum--;
        }

    }
}
