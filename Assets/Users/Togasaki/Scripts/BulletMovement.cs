using UnityEngine;

public class BulletMovement : MonoBehaviour
{

    [SerializeField] private Transform OriginBulletLocation;


    private void Start()
    {
        gameObject.transform.position = OriginBulletLocation.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Wall")
        {
            transform.position = OriginBulletLocation.position;
        }

    }

}
