using UnityEngine;
using System.Collections;

public class CharaMove : MonoBehaviour
{
    //作成したJoystick
    [SerializeField]
    private Joystick _joystick = null;

    //移動速度
    [SerializeField]
    public float speed = 5.0f;

    float moveX = 0f;
    float moveZ = 0f;

    private Vector3 latestPos;  //前回のPosition

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

        // 移動方向にキャラクターが向くようにする
        Vector3 diff = transform.position - latestPos;   //前回からどこに進んだかをベクトルで取得
        latestPos = transform.position;  //前回のPositionの更新

        //ベクトルの大きさが0.01以上の時に向きを変える処理をする
        if (diff.magnitude > 0.01f && diff.y == 0)
        {
            transform.rotation = Quaternion.LookRotation(diff); //向きを変更する
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            // 一時的に無効化
            // Debug.Log("Hit");
            // RoundManager.RoundMove = true;
            // StartCoroutine(Move());
        }
    }

    /// <summary>
    /// CharacterController一時的に無効にし、プレイヤーのポジションをリセットするコルーチン
    /// </summary>
    /// <returns></returns>
    IEnumerator Move()
    {
        yield return new WaitForFixedUpdate();
        controller.enabled = false;
        this.gameObject.transform.position = Vector3.zero;
        controller.enabled = true;

    }
}
