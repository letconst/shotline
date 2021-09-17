using UnityEngine;

public class BulletLifeSpan : MonoBehaviour
{
    //���W�ۑ��p�ϐ�
    Vector3 beforePosition;

    private void Update()
    {
        LifeSpanFunc();
    }

    void LifeSpanFunc()
    {
        Vector3 nowPosition = this.transform.position;

        if(beforePosition != null && nowPosition == beforePosition)
        {
            Destroy(gameObject);
        }

        beforePosition = nowPosition;
    }

}
