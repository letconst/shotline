using UnityEngine;

public class ShieldMovement : MonoBehaviour
{

    public static int ShieldLimit = 4;


    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Bullet")
        {
            //�����蔻��ɓ������̂�Bullet�������ꍇShieldLimit���}�C�i�X�P
            ShieldLimit--;

            if (ShieldLimit < 0)
            {
                ShieldLimit = 4;
                Destroy(gameObject);
            }
        }


    }

}
