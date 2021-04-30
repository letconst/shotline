using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMovement : MonoBehaviour
{
	void OnCollisionEnter(Collision collision)
	{
		// 衝突した相手にWallタグが付いているとき
		if (collision.gameObject.tag == "Wall")
		{
			Destroy(gameObject);
		}
	}

}
