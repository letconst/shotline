using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    //Bullet prefab
    public GameObject Bullet;

    //弾丸発射点
    public Transform Muzzle;

    //弾速
    public float Speed = 1000;


    public void Fire()
    {
        //ボタンをクリックでBulletを生成
        GameObject Bullets = Instantiate(Bullet) as GameObject;

        Vector3 force;
        force = this.gameObject.transform.forward * Speed;

        ////Rigidbodyに力を加えて発射
        Bullets.GetComponent<Rigidbody>().AddForce(force);

        Bullets.transform.position = Muzzle.position;


    }

}
