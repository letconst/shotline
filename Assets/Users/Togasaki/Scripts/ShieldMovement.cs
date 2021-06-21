using UnityEngine;

public class ShieldMovement : MonoBehaviour
{

    private int ShieldLimit = 10;

    private void Start()
    {
        ShieldLimit = 10;
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Bullet")
        {
            //当たり判定に入ったのがBulletだった場合ShieldLimitをマイナス１
            ShieldLimit--;

            if (ShieldLimit <= 0)
            {
                Destroy(gameObject);
            }
        }


    }

}
