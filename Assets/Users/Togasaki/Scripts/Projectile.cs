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

    //弾丸の速さ
    public float Speed = 1000;

    Vector3[] FingerPositions;

    LineRenderer Line;

    int index = 0;

    bool flag;


    //GameObject Bullets;

    private void Start()
    {
       //Bullets = Instantiate(Bullet) as GameObject;
    }

    void Update()
    {
        if (Line == null)
        {
            Line = GameObject.FindGameObjectWithTag("ShotLine").GetComponent<LineRenderer>();
        }


        if (Input.GetMouseButtonUp(0))
        {
            flag = true;
        }

        if (Line != null && Line.enabled && flag)
        {
            Vector3[] FingerPositions = ShotLineDrawer.GetFingerPositions();

            if (index < FingerPositions.Length)
            {
                //GameObject Bullets = Instantiate(Bullet) as GameObject;

                //ここ
                FingerPositions[index] *= Time.deltaTime;


                Bullet.transform.position = FingerPositions[index];

                index++;

            }
            else
            {
                ShotLineDrawer.ClearLine();
                index = 0;
                flag = false;

            }
        }
    }

    public void Fire()
    {
        flag = true;
    }
}
