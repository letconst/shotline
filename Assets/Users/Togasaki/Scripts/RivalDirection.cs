using UnityEngine;
using UnityEngine.UI;

public class RivalDirection : MonoBehaviour
{
    [SerializeField, Header("矢印イメージ")]
    private Image Arrow;

    [SerializeField, Header("MAX位置X")]
    private float maxPosX = 880;

    [SerializeField, Header("MAX位置Y")]
    private float maxPosY = 640;

    //プレイヤー格納変数
    GameObject PlayerCharacter;

    //対戦相手格納変数
    GameObject RivalCharacter;

    //イメージのrectTransform
    RectTransform arrowRT;

    //rectTransform位置
    float posX;
    float posY;

    //位置の割合
    float ratio;

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

        if (deg >= -45 && deg < 0)
        {
            ratio = Mathf.InverseLerp(-45, 0, deg);
            arrowRT.localPosition = new Vector3(-maxPosX, Mathf.Lerp(maxPosY, 0, ratio), 0);
        }
        else if (deg >= 0 && deg < 45)
        {
            ratio = Mathf.InverseLerp(0, 45, deg);
            arrowRT.localPosition = new Vector3(-maxPosX, Mathf.Lerp(0, -maxPosY, ratio), 0);
        }
        else if (deg >= 45 && deg < 90)
        {
            ratio = Mathf.InverseLerp(45, 90, deg);
            arrowRT.localPosition = new Vector3(Mathf.Lerp(-maxPosX, 0, ratio), -maxPosY, 0);
        }
        else if (deg >= 90 && deg < 135)
        {
            ratio = Mathf.InverseLerp(90, 135, deg);
            arrowRT.localPosition = new Vector3(Mathf.Lerp(0, maxPosX, ratio), -maxPosY, 0);
        }
        else if (deg >= 135 && deg < 180)
        {
            ratio = Mathf.InverseLerp(135, 180, deg);
            arrowRT.localPosition = new Vector3(maxPosX, Mathf.Lerp(-maxPosY, 0, ratio), 0);
        }
        else if (deg >= -180 && deg < -135)
        {
            ratio = Mathf.InverseLerp(-180, -135, deg);
            arrowRT.localPosition = new Vector3(maxPosX, Mathf.Lerp(0, maxPosY, ratio), 0);
        }
        else if (deg >= -135 && deg < -90)
        {
            ratio = Mathf.InverseLerp(-135, -90, deg);
            arrowRT.localPosition = new Vector3(Mathf.Lerp(maxPosX, 0, ratio), maxPosY, 0);
        }
        else if (deg >= -90 && deg < -45)
        {
            ratio = Mathf.InverseLerp(-90, -45, deg);
            arrowRT.localPosition = new Vector3(Mathf.Lerp(0, -maxPosX, ratio), maxPosY, 0);
        }

    }

    
}
