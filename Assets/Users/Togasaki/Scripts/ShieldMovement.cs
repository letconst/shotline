using UnityEngine;

public class ShieldMovement : MonoBehaviour
{

    public static int ShieldLimit = 4;


    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Bullet")
        {
            //当たり判定に入ったのがBulletだった場合ShieldLimitをマイナス１
            ShieldLimit--;

            if (ShieldLimit < 0)
            {
                ShieldLimit = 4;
                Destroy(gameObject);
            }
        }


    }

}
