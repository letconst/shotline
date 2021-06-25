using UnityEngine;
using System.Collections;

public class CharaMove : MonoBehaviour
{
    //Joystickプレハブ
    [SerializeField]
    private Joystick _joystick = null;

    //移動速度
    [SerializeField]
    private float _speed = 5.0f; //キャラクターの移動速度

    private float _moveX = 0f; //キャラクターのX方向への移動速度
    private float _moveZ = 0f; //キャラクターのY方向への移動速度

    private Vector3 _latestPos;  //前回のPosition

    CharacterController controller; //CharacterControllerの読み込み

    void Start()
    {
        controller = GetComponent<CharacterController>(); //CharacterControllerの取得
    }

    void Update()
    {
        if (RoundManager.RoundMove == true)
        {
            return; //RoundMoveがtrueになると操作不能に
        }

        _moveX = _joystick.Position.x * _speed; //JoystickのPositionに_speedをかけて、_moveXに代入
        _moveZ = _joystick.Position.y * _speed; //JoystickのPositionに_speedをかけて、_moveYに代入
        Vector3 direction = new Vector3(_moveX, 0, _moveZ);

        controller.SimpleMove(direction);

        // 移動方向にキャラクターが向くようにする
        Vector3 diff = transform.position - _latestPos;     //前回からどこに進んだかをベクトルで取得
        _latestPos = transform.position;                    //前回のPositionの更新

        //ベクトルの大きさが0.01以上の時に向きを変える処理をする
        if (diff.magnitude > 0.01f)
        {
            transform.rotation = Quaternion.LookRotation(diff); //向きを変更する
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall") //特定のTagの付いたオブジェクトを判別
        {
            Debug.Log("Hit");
            RoundManager.RoundMove = true; //RoundManagerのRoundMoveをtrueに
            StartCoroutine(Move()); //コルーチンの呼び出し
        }
    }

    /// <summary>
    /// CharacterController一時的に無効にし、プレイヤーのポジションをリセットするコルーチン
    /// </summary>
    /// <returns></returns>
    IEnumerator Move()
    {
        yield return new WaitForFixedUpdate();
        controller.enabled = false; // CharacterControllerを無効に
        this.gameObject.transform.position = Vector3.zero; //プレイヤーのポジションを(0,0,0)に
        controller.enabled = true; // CharacterControllerを有効に
    }
}