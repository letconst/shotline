using UnityEngine;

public class BulletMovement : MonoBehaviour
{

    public static bool BulletBreak = false;

    [SerializeField] private Transform OriginBulletLocation;


    private void Start()
    {
        gameObject.transform.position = OriginBulletLocation.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Wall" || other.gameObject.tag == "Shield")
        {
            BulletBreak = true;
            transform.position = OriginBulletLocation.position;
        }

    }

}
