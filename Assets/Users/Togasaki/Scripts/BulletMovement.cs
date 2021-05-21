using UnityEngine;

public class BulletMovement : MonoBehaviour
{

    [SerializeField] private Transform OriginBulletLocation;


    void OnCollisionEnter(Collision collision)
	{


		// 衝突した相手にWallタグが付いているとき
		if (collision.gameObject.tag == "Wall")
		{
            transform.position = OriginBulletLocation.position;
        }
    }

}
