using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMovement : MonoBehaviour
{

    public Transform OriginBulletLocation;


    private void Update()
    {
        if(true)
        {
            transform.position = OriginBulletLocation.position;
        }
    }

    void OnCollisionEnter(Collision collision)
	{


		// 衝突した相手にWallタグが付いているとき
		if (collision.gameObject.tag == "Wall")
		{
			Destroy(gameObject);
		}
	}

}
