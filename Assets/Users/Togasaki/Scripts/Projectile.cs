using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    /*
     
    "Projectile"クラスの概要
     
    もしボタンが押されたらShotLineDrawerクラスの座標を取得し、その座標をなぞるように弾丸を発射する。
    変数"Muzzle"に弾丸の発射点となる場所を指定できる。
    変数"Speed"に弾丸の速さを指定できる。
     
     */

    //弾丸のprefab
    public GameObject Bullet;

    //弾丸発射点
    public Transform Muzzle;

    //弾丸の速さ、1から下は弾速Max、値が増えると弾速は遅くなる
    public int SavedSpeed = 1;
    //Speed計算用の変数
    int CalSpeed;

    //射線の座標をいれる配列
    Vector3[] FingerPositions;

    //射線の変数
    LineRenderer Line;

    //Updateで使う用のint
    int index = 0;

    //ボタンが押された用のflag
    bool flag;

    private void Start()
    {
        CalSpeed = SavedSpeed;
    }

    void Update()
    {

        //もし射線が空だったら
        if (Line == null)
        {
            Line = GameObject.FindGameObjectWithTag("ShotLine").GetComponent<LineRenderer>();
        }

        //もし射線があってflagがtrueだったら
        if (Line != null && Line.enabled && flag)
        {
            //配列に射線の全座標をいれる
            FingerPositions = ShotLineDrawer.GetFingerPositions();

            //弾が進んだ長さがindexの数値より小さかったら
            if (index < FingerPositions.Length)
            {
                //FingerPositionの場所に出現
                Bullet.transform.position = FingerPositions[index];

                //CalSpeedの値を1減らす
                CalSpeed--;

                //もしCalSpeedの値が0以下だったら
                if (CalSpeed <= 0)
                {
                    //CalSpeedを戻す
                    CalSpeed = SavedSpeed;
                    //次の座標の配列に移行
                    index++;
                }
            }
            //弾が進んだ長さがindexになったら
            else
            {
                ShotLineDrawer.ClearLine();
                index = 0;
                flag = false;
            }

        }
    }

    //射撃ボタン、flagをtrueに
    public void Fire()
    {
        flag = true;
    }
}
