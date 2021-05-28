using UnityEngine;
using System.Collections;

public class CharaMove : MonoBehaviour
{

    //作成したJoystick
    [SerializeField]
    private Joystick _joystick = null;

    //移動速度
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

            // プレイヤーポジションのリセット
            this.transform.position = new Vector3(0, 0, 0);
        }

    }
}