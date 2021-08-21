using UnityEngine;
using UnityEngine.UI;

public class RivalDirection : MonoBehaviour
{
    [SerializeField, Header("���C���[�W")]
    private Image Arrow;

    [SerializeField, Header("MAX�ʒuX")]
    private float maxPosX = 880;

    [SerializeField, Header("MAX�ʒuY")]
    private float maxPosY = 640;

    //�v���C���[�i�[�ϐ�
    GameObject PlayerCharacter;

    //�ΐ푊��i�[�ϐ�
    GameObject RivalCharacter;

    //�C���[�W��rectTransform
    RectTransform arrowRT;

    //rectTransform�ʒu
    float posX;
    float posY;

    //�ʒu�̊���
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
