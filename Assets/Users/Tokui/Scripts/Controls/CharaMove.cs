using UnityEngine;
using System.Collections;

public class CharaMove : MonoBehaviour
{
    //作成したJoystick
    [SerializeField]
    private Joystick _joystick = null;

    //移動速度
    [SerializeField]
    public float speed = 10.0f;

    float moveX = 0f;
    float moveZ = 0f;


    CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (RoundManager.RoundMove == true)
        {
            return;
        }

        moveX = _joystick.Position.x * speed;
        moveZ = _joystick.Position.y * speed;
        Vector3 direction = new Vector3(moveX, 0, moveZ);

        controller.SimpleMove(direction);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            Debug.Log("Hit");
            RoundManager.RoundMove = true;

            // プレイヤーポジションのリセット
            //Debug.Log(this.transform.position);
            //this.transform.position = new Vector3(0, 0, 0);
            //Debug.Log(this.transform.position);

            // controller.Move(Vector3.zero);

            StartCoroutine(Move());
        }
    }

    IEnumerator Move()
    {
        yield return new WaitForFixedUpdate();
        controller.enabled = false;
        this.gameObject.transform.position = Vector3.zero;
        controller.enabled = true;

    }
}