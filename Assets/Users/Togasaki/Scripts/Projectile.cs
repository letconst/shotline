using UnityEngine;
using System.Collections.Generic;


public class BulletInfo
{
    //弾丸のprefab
    public GameObject Bullet;

    //射線の座標をいれる配列
    public Vector3[] FP;

    //射線の現在座標用int
    public int index;

    //個々の弾のスピード
    public float Speed;

    public readonly LineData LineData;


    public BulletInfo(GameObject bullet, Vector3[] fp, int ind, float spd, LineData lineData)
    {
        Bullet   = bullet;
        FP       = fp;
        index    = ind;
        Speed    = spd;
        LineData = lineData;
    }
}


public class Projectile : MonoBehaviour
{
    /*

    "Projectile"クラスの概要

    もしボタンが押されたらShotLineDrawerクラスの座標を取得し、その座標をなぞるように弾丸を発射する。
    変数"Speed"に弾丸の速さを指定できる。

     */

    //変数ゾーン///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    //リストに弾の情報を
    List<BulletInfo> BulletList = new List<BulletInfo>();

    //はやさ
    private float ActSpeed;
    private float OriginSpeed = 10;
    private float BBSpeed = 8;

    //弾のスケール
    private float BaseScale = 1f;
    public static float ScaleRatio = 1f;

    //射線の変数
    public static LineData currentLineData;

    //一回だけ射線の座標を取得
    public static bool One = false;

    //for用
    private int i = 0;

    //射線用
    private bool flag = true;

    private BulletMovement BM;

    /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////


    private void Start()
    {
        ItemManager.ShotBtn.onClick.AddListener(() => Fire());
        BulletList      = new List<BulletInfo>();
        ActSpeed        = OriginSpeed;
        ScaleRatio      = 1;
        currentLineData = null;
    }

    void Update()
    {
        LineAppear();
    }


    //射線に沿って弾丸を移動させる処理
    private void LineAppear()
    {
        currentLineData = ShotLineDrawer.DrawingData;

        //ラインが引かれていたら
        if (currentLineData != null && currentLineData.Renderer.enabled && One)
        {
            Vector3[] FingerPositions = ShotLineUtil.GetFingerPositions(currentLineData);

            //弾生成
            GameObject BI = Instantiate(MainGameController.bulletPrefab, FingerPositions[0], Quaternion.identity);
            BI.AddComponent<BulletMovement>();

            //射撃SEを鳴らしている
            SoundManager.Instance.PlaySE(SELabel.Shot);

            BI.transform.localScale = new Vector3(BaseScale * ScaleRatio, BaseScale * ScaleRatio, BaseScale * ScaleRatio);


            //配列に射線の全座標とそれに対応する弾丸をいれる
            BulletList.Add(new BulletInfo(BI, ShotLineUtil.GetFingerPositions(currentLineData), 0, ActSpeed, currentLineData));

            if (BulletList.Count > 1)
            {
                flag = false;
            }

            One = false;

        }


        //弾を実際に動かす部分
        if (BulletList.Count > 0)
        {

            for (i = 0; i < BulletList.Count; i++)
            {

                BM = BulletList[i].Bullet.GetComponent<BulletMovement>();

                //現在の座標を変更できるように変数化
                BulletInfo currentP = BulletList[i];

                //もし射線の長さが最後だったら
                if (BulletList[i].index == BulletList[i].FP.Length - 1)
                {
                    BulletList[i].Bullet.transform.position = Vector3.MoveTowards(BulletList[i].FP[BulletList[i].FP.Length - 2], BulletList[i].FP[BulletList[i].FP.Length - 1], BulletList[i].Speed * Time.deltaTime);
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
                        BulletList[i].Bullet.transform.position = Vector3.MoveTowards(BulletList[i].Bullet.transform.position, BulletList[i].FP[BulletList[i].index + 1], BulletList[i].Speed * Time.deltaTime);
                    }
                }

                //もし弾が次の位置まで到達したら、その次の位置を読み込む
                if (BulletList[i].Bullet.transform.position == BulletList[i].FP[BulletList[i].index + 1])
                {
                    currentP.index++;
                    BulletList[i] = currentP;
                }

                //弾が動き終わったら、もしくは壁かシールドに当たったら
                if (BulletList[i].index == BulletList[i].FP.Length - 1||BM.BBOn)
                {

                    if (BulletList.Count == i+1)
                    {
                        flag = true;
                    }

                    ShotLineUtil.FreeLineData(BulletList[i].LineData);
                    BM.BBOn = false;
                    Destroy(BulletList[i].Bullet);
                    BulletList.RemoveAt(i);
                }

            }

        }
    }

    //射撃ボタンを押したとき
    public void Fire()
    {

        ScaleRatio = 1f;
        ActSpeed = OriginSpeed;

        if (BigBullet.BBOn && BigBullet.ClickBB)
        {
            //スケール
            ScaleRatio = 1.5f;
            //スピードを変える
            ActSpeed = BBSpeed;

            BigBullet.ClickBB = false;
        }

        currentLineData = ShotLineDrawer.DrawingData;

        // 射線が固定されていなければ
        if (currentLineData is {IsFixed: false})
        {
            //射線の固定
            ShotLineUtil.FixLine(currentLineData);

            //一回だけ座標を取得用
            One = true;

        }

    }


}
