using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMovement : MonoBehaviour
{
    public static bool BBOn = false;

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Wall" || other.gameObject.tag == "Shield")
        {
            BBOn = true;
        }

    }

}
