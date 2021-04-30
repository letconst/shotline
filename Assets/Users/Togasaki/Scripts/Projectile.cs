using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    //Bullet prefab
    public GameObject Bullet;

    //�e�۔��˓_
    public Transform Muzzle;

    //�e��
    public float Speed = 1000;


    public void Fire()
    {
        //�{�^�����N���b�N��Bullet�𐶐�
        GameObject Bullets = Instantiate(Bullet) as GameObject;

        Vector3 force;
        force = this.gameObject.transform.forward * Speed;

        ////Rigidbody�ɗ͂������Ĕ���
        Bullets.GetComponent<Rigidbody>().AddForce(force);

        Bullets.transform.position = Muzzle.position;


    }

}
