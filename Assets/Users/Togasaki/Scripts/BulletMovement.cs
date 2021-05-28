using UnityEngine;

public class BulletMovement : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Wall" || other.gameObject.tag == "Shield")
        {
            Destroy(gameObject);
        }

    }

}
