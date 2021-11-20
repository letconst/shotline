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

    //ラインデータ
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

public class Projectile : SingletonMonoBehaviour<Projectile>, IManagedMethod
{
    /*

    "Projectile"クラスの概要

    もしボタンが押されたらShotLineDrawerクラスの座標を取得し、その座標をなぞるように弾丸を発射する。
    変数"ActSpeed"に弾丸の速さを指定できる。

     */

    //リストに弾の情報を
    List<BulletInfo> BulletList = new List<BulletInfo>();

    //はやさ
    private       float ActSpeed;
    public static float OriginSpeed = 10;
    private       float BBSpeed     = 8;

    //弾のスケール
    private       float BaseScale  = 1f;
    public static float ScaleRatio = 1f;

    //射線の変数
    public static LineData currentLineData;

    //一回だけ射線の座標を取得
    public static bool One = false;

    private bool _isQueuedDestroyAll;

    //プレイヤー格納変数
    GameObject PlayerCharacter;

    [SerializeField, Header("マズル位置")]
    private Transform muzzleFlashPoint;

    [SerializeField, Header("弾丸発射エフェクト")]
    private GameObject muzzleFlashEffect;

    [SerializeField, Header("弾丸消滅エフェクト")]
    private GameObject disappearanceBullet;

    //省略する距離
    float refTime = 0.05f;

    public void ManagedStart()
    {
        PlayerCharacter = GameObject.FindGameObjectWithTag("Player");
        ItemManager.ShotBtn.onClick.AddListener(() => Fire());
        BulletList      = new List<BulletInfo>();
        ActSpeed        = OriginSpeed;
        BBSpeed         = OriginSpeed / 100 * 80;
        ScaleRatio      = 1;
        currentLineData = null;
        One             = false;
    }

    public void ManagedUpdate()
    {
        // 全破棄がキューされてたらそれ実行
        if (_isQueuedDestroyAll)
        {
            _isQueuedDestroyAll = false;

            InnerDestroyAllBullets();

            return;
        }

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

            One = false;
            BigBullet.ClickBB = false;

        }

        //弾を実際に動かす部分
        if (BulletList.Count > 0)
        {
            for (int i = 0; i < BulletList.Count; i++)
            {
                if (BulletList[i].Bullet == null)
                {
                    continue;
                }

                //BulletMovement取得
                BulletMovement BM = BulletList[i].Bullet.GetComponent<BulletMovement>();

                //現在の座標から次の座標の方向を向く
                Vector3 diff = BulletList[i].FP[BulletList[i].index + 1] - BulletList[i].Bullet.transform.position;

                if (diff != new Vector3(0, 0, 0))
                {
                    BulletList[i].Bullet.transform.rotation = Quaternion.LookRotation(diff);
                }

                //次の座標が指定の距離以下なら省略
                if (diff.magnitude < refTime && BulletList[i].index < BulletList[i].FP.Length - 5)
                {
                    while(true)
                    {
                        Vector3 diffp2 = BulletList[i].FP[BulletList[i].index + 1] - BulletList[i].Bullet.transform.position;
                        if (diffp2.magnitude < refTime)
                        {
                            BulletList[i].Bullet.transform.position = BulletList[i].FP[BulletList[i].index + 1];
                            //もし弾が次の位置まで到達したら、その次の位置を読み込む
                            if (BulletList[i].Bullet.transform.position == BulletList[i].FP[BulletList[i].index + 1])
                            {
                                BulletList[i].index++;
                            }
                        }
                        else
                        {
                            //現在の射線の位置から次の射線の位置まで移動
                            float ratio = BulletList[i].Speed * Time.deltaTime;
                            BulletList[i].Bullet.transform.position = Vector3.MoveTowards(BulletList[i].Bullet.transform.position, BulletList[i].FP[BulletList[i].index + 1], ratio);

                            //もし弾が次の位置まで到達したら、その次の位置を読み込む
                            if (BulletList[i].Bullet.transform.position == BulletList[i].FP[BulletList[i].index + 1])
                            {
                                BulletList[i].index++;
                            }
                            break;
                        }
                    }
                }
                else
                {
                    //現在の射線の位置から次の射線の位置まで移動
                    float ratio = BulletList[i].Speed * Time.deltaTime;
                    BulletList[i].Bullet.transform.position = Vector3.MoveTowards(BulletList[i].Bullet.transform.position, BulletList[i].FP[BulletList[i].index + 1], ratio);

                    //もし弾が次の位置まで到達したら、その次の位置を読み込む
                    if (BulletList[i].Bullet.transform.position == BulletList[i].FP[BulletList[i].index + 1])
                    {
                        BulletList[i].index++;
                    }
                }

                //弾が動き終わったら、もしくは壁かシールドに当たったら
                if (BulletList[i].index == BulletList[i].FP.Length - 1 || BM.BBOn)
                {
                    ShotLineUtil.FreeLineData(BulletList[i].LineData);
                    BM.BBOn = false;
                    SoundManager.Instance.PlaySE(SELabel.electric_chain,0.5f);
                    Instantiate(disappearanceBullet, BulletList[i].Bullet.transform.position, Quaternion.identity);
                    Destroy(BulletList[i].Bullet);
                    //BulletList.RemoveAt(i);
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
            ShotLineDrawer.IsLineCreated = false;

            //一回だけ座標を取得用
            One = true;

            //ゲージを消費
            LineGaugeController.Clicked();

            //プレイヤーが射線の方向を向く
            PlayerCharacter.transform.LookAt(currentLineData.FingerPositions[1],Vector3.up);
            PlayerCharacter.transform.rotation = new Quaternion(0, PlayerCharacter.transform.rotation.y, 0, PlayerCharacter.transform.rotation.w);

            //射撃エフェクト
            Instantiate(muzzleFlashEffect, muzzleFlashPoint.position, PlayerCharacter.transform.rotation);

            //リニアドロー
            if (LinearDraw._isLinearDraw)
            {
                ShotLineDrawer._firstLinearDraw = true;
            }
        }
    }

    private void InnerDestroyAllBullets()
    {
        foreach (BulletInfo bullet in BulletList)
        {
            Destroy(bullet.Bullet);
        }

        BulletList.Clear();
    }

    public static void DestroyAllBullets()
    {
        Instance._isQueuedDestroyAll = true;
    }
}
