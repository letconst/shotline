using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    /*
     
    "Projectile"クラスの概要
     
    もしボタンが押されたらShotLineDrawerクラスの座標を取得し、その座標をなぞるように弾丸を発射する。
    変数"Speed"に弾丸の速さを指定できる。
     
     */

    //弾丸のprefab
    [SerializeField] private GameObject Bullet;

    //弾丸の速さ、1から下は弾速Max、値が増えると弾速は遅くなる
    [SerializeField] private int SavedSpeed = 1;
    //Speed計算用の変数
    int CalSpeed;
    
    //射線の座標をいれる配列
    Vector3[] FingerPositions;

    //射線の変数
    LineRenderer Line;

    ////Updateで使う用のint
    int index;

    //ラインの全長
    float AllDistance = 0f;
    
    //点と点の長さ
    float distance = 0f;
    float Pointdistance = 0f;
    int i;

    //ボタンが押された用のflag
    bool flag;

    float CalDistance;

    float Calratio = 1f;

    //全体からの点と点の距離の割合
    float ratio = 0f;


    void Update()
    {
        //もし射線が空だったら
        if (Line == null)
        {
            Line = GameObject.FindGameObjectWithTag("ShotLine").GetComponent<LineRenderer>();
        }

        //ラインが引かれていたら
        if (Line != null && Line.enabled)
        {
            i = 0;
            distance = 0f;
            AllDistance = 0f;

            //配列に射線の全座標をいれる
            FingerPositions = ShotLineDrawer.GetFingerPositions();

            //ラインの全長
            while (i + 1 < FingerPositions.Length)
            {
                distance = Vector3.Distance(FingerPositions[i], FingerPositions[i + 1]);
                i++;
                AllDistance = AllDistance + distance;
            }

            ratio = CalDistance / AllDistance;
            float projectileDistance;

            Bullet.transform.position = NG.Vector3Ext.MultiLerp(FingerPositions, ratio, out projectileDistance);

            CalDistance += Pointdistance;
            //Debug.Log(Pointdistance);

            Calratio = CalDistance;
        }

        //もしラインがあってボタンが押されたら
        if (Line != null && Line.enabled && flag)
        {
            ////indexの数値がラインの全長より小さかったら
            //if (index < FingerPositions.Length)
            //{


            //    //Bullet.transform.position = Vector3.Lerp(FingerPositions[index], FingerPositions[index + 1], ratio);

            //    if (Calratio > 1)
            //    {
            //        ratio = Calratio - 1;
            //        Calratio = Mathf.Floor(Calratio);
            //        index++;
            //    }

            //    //現在の点と次の点の距離
            //    //if (index == FingerPositions.Length - 1)
            //    //{
            //    //    Pointdistance = Vector3.Distance(FingerPositions[FingerPositions.Length - 2], FingerPositions[FingerPositions.Length - 1]);
            //    //}
            //    //else
            //    //{
            //    //    Pointdistance += Vector3.Distance(FingerPositions[index], FingerPositions[index + 1]);
            //    //}

            //    //    //もし現在の点がラインの長さと同じなら
            //    //    if (index == FingerPositions.Length - 1)
            //    //    {
            //    //        var vec = Vector3.Lerp(FingerPositions[FingerPositions.Length - 2], FingerPositions[FingerPositions.Length - 1], ratio);
            //    //        //弾の位置を代入
            //    //        Bullet.transform.position = vec;

            //    //    }
            //    //    else
            //    //    {
            //    //        //現在の点から次の点
            //    //        var vec = Vector3.Lerp(FingerPositions[index], FingerPositions[index + 1], ratio);
            //    //        //弾の位置を代入
            //    //        Bullet.transform.position = vec;

            //    //    }

            //    //CalSpeedの値を1減らす
            //    CalSpeed--;

            //    //もしCalSpeedの値が0以下だったら
            //    if (CalSpeed <= 0)
            //    {
            //        //CalSpeedを戻す
            //        //CalSpeed = SavedSpeed;
            //        //次の座標の配列に移行
            //        index++;
            //    }
            //}
            ////index=ラインの全長になったら
            //else
            //{
            //    ShotLineDrawer.ClearLine();
            //    index = 0;
            //    flag = false;
            //}
        }
    }

    //射撃ボタン、flagをtrueに
    [SerializeField]
    private void Fire()
    {
        flag = true;
    }
}
