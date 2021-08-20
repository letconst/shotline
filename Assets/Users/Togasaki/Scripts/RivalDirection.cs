using UnityEngine;
using UnityEngine.UI;

public class RivalDirection : MonoBehaviour
{
    [SerializeField, Header("矢印イメージ")]
    private Image Arrow;

    //プレイヤー格納変数
    GameObject PlayerCharacter;

    //対戦相手格納変数
    GameObject RivalCharacter;

    //イメージのrectTransform
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
