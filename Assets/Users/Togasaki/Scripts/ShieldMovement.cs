using UnityEngine;

public class ShieldMovement : MonoBehaviour
{
    //OriginShieldLocation���`
    [SerializeField] private Transform OriginShieldLocation;


    public static int ShieldLimit = 4;


    private void Start()
    {
        gameObject.transform.position = OriginShieldLocation.position;

    }

    private void Update()
    {
        if (Shield.ShiOn == false)
        {
            gameObject.transform.position = OriginShieldLocation.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Enter");

        if (other.gameObject.tag == "Bullet")
        {
            //�����蔻��ɓ������̂�Bullet�������ꍇShieldLimit���}�C�i�X�P
            ShieldLimit--;
            if (ShieldLimit < 0)
            {
                ShieldLimit = 4;
                Shield.ShiOn = false;
            }
        }

    }



}
