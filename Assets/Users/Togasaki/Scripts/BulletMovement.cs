using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMovement : MonoBehaviour
{
	void OnCollisionEnter(Collision collision)
	{
		// �Փ˂��������Wall�^�O���t���Ă���Ƃ�
		if (collision.gameObject.tag == "Wall")
		{
			Destroy(gameObject);
		}
	}

}
