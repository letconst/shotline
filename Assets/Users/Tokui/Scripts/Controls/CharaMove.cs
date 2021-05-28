using UnityEngine;
using System.Collections;

public class CharaMove : MonoBehaviour
{

    //�쐬����Joystick
    [SerializeField]
    private Joystick _joystick = null;

    //�ړ����x
    [SerializeField]
    private float SPEED = 0.1f;

    private void Update()
    {
        if (RoundManager.RoundMove == true)
        {
            return;
        }
        Vector3 pos = transform.position;

        pos.x += _joystick.Position.x * SPEED;
        pos.z += _joystick.Position.y * SPEED;

        transform.position = pos;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            RoundManager.RoundMove = true;

            // �v���C���[�|�W�V�����̃��Z�b�g
            this.transform.position = new Vector3(0, 0, 0);
        }

    }
}