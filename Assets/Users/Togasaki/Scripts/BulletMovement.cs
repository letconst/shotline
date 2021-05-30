using UnityEngine;

public class BulletMovement : MonoBehaviour
{
    public bool BBOn = false;

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Wall" || other.gameObject.tag == "Shield")
        {
            BBOn = true;
        }

    }

}
