using UnityEngine;

public class ShieldMovement : MonoBehaviour
{
    //OriginShieldLocation‚ð’è‹`
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
        if (other.gameObject.tag == "Bullet")
        {
            ShieldLimit--;
            if (ShieldLimit < 0)
            {
                Shield.ShiOn = false;
            }
        }

    }



}
