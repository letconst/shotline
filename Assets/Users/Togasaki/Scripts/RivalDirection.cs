using UnityEngine;
using UnityEngine.UI;

public class RivalDirection : MonoBehaviour
{
    [SerializeField, Header("ξσC[W")]
    private Image Arrow;

    //vC[i[Ο
    GameObject PlayerCharacter;

    //Ξνθi[Ο
    GameObject RivalCharacter;

    //C[WΜrectTransform
    RectTransform arrowRT;

    void Start()
    {
        PlayerCharacter = GameObject.FindGameObjectWithTag("Player");
        RivalCharacter = GameObject.FindGameObjectWithTag("Rival");
        arrowRT = Arrow.rectTransform;

    }

    void Update()
    {
        Direction();
    }

    private void Direction()
    {
        float radian = Mathf.Atan2(RivalCharacter.transform.position.z - PlayerCharacter.transform.position.z, RivalCharacter.transform.position.x - PlayerCharacter.transform.position.x);
        float deg = radian * (180 / 3.14f);
        arrowRT.rotation = Quaternion.Euler(0, 0, deg - 90);

    }

}
