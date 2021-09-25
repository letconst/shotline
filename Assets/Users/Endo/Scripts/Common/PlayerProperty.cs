using UnityEngine;

public class PlayerProperty : MonoBehaviour
{
    [SerializeField]
    private GameObject drawLineGun;

    [SerializeField]
    private GameObject linearLineGun;

    public GameObject DrawLineGun   => drawLineGun;
    public GameObject LinearLineGun => linearLineGun;
}
