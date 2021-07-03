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

        if (other.CompareTag("Bullet") || other.CompareTag("RivalBullet"))
        {
            //�����蔻��ɓ������̂�Bullet�������ꍇShieldLimit���}�C�i�X�P
            ShieldLimit--;

            if (ShieldLimit <= 0)
            {
                Destroy(gameObject);
            }
        }


    }

}
