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
    public GameObject Bullet;

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

    //弾の進行具合(Lerpの第三引数)
    float time = 0;


    //private void Start()
    //{
    //    CalSpeed = SavedSpeed;
    //}

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

            //indexの数値よがラインの全長より小さかったら
            if (index < FingerPositions.Length)
            {

                time += Time.deltaTime;

                if (index == FingerPositions.Length - 1)
                {
                    var vec = Vector3.Lerp(FingerPositions[FingerPositions.Length - 2], FingerPositions[FingerPositions.Length - 1], time);
                    //弾の位置を代入
                    Bullet.transform.position = vec;
                }
                else
                {
                    //現在の点から次の点
                    var vec = Vector3.Lerp(FingerPositions[index], FingerPositions[index + 1], time);
                    //弾の位置を代入
                    Bullet.transform.position = vec;
                }

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
            //index=ラインの全長になったら
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
