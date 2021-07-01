using UnityEngine;
using System.Collections;
using UniRx;

public class CharaMove : MonoBehaviour
{
    //Joystickプレハブ
    private Joystick _joystick = null;

    //移動速度
    [SerializeField]
    private float _speed = 5.0f; //キャラクターの移動速度

    public float speedRatio;

    // 弾が当たったかどうかのフラグ
    public bool Thruster;

    private float _moveX = 0f; //キャラクターのX方向への移動速度
    private float _moveZ = 0f; //キャラクターのY方向への移動速度

    private Vector3 _latestPos; //前回のPosition

    CharacterController controller; //CharacterControllerの読み込み

    public float CurrentSpeed => _speed * speedRatio;

    void Start()
    {
        speedRatio = 1;
        _joystick  = GameObject.FindGameObjectWithTag("Joystick").GetComponent<Joystick>();
        controller = GetComponent<CharacterController>(); //CharacterControllerの取得
    }

    void Update()
    {
        //無敵フラグが立っているとき
        //移動処理を行わない
        if (Thruster == true)
        {
            return;
        }

        if (RoundManager.RoundMove == true)
        {
            return; //RoundMoveがtrueになると操作不能に
        }

        if (!MainGameController.isControllable)
        {
            controller.SimpleMove(Vector3.zero);

            return;
        }

        _moveX = _joystick.Position.x * CurrentSpeed; //JoystickのPositionに_speedをかけて、_moveXに代入
        _moveZ = _joystick.Position.y * CurrentSpeed; //JoystickのPositionに_speedをかけて、_moveYに代入

        // 2pはカメラを反転させるため、移動方向も逆に
        if (!NetworkManager.IsOwner)
        {
            _moveX = -_moveX;
            _moveZ = -_moveZ;
        }

        Vector3 direction = new Vector3(_moveX, 0, _moveZ);

        controller.SimpleMove(direction);

        // 移動方向にキャラクターが向くようにする
        Vector3 diff = transform.position - _latestPos; //前回からどこに進んだかをベクトルで取得
        _latestPos = transform.position;                //前回のPositionの更新

        //ベクトルの大きさが0.01以上の時に向きを変える処理をする
        if (diff.magnitude > 0.01f && diff.y == 0)
        {
            transform.rotation = Quaternion.LookRotation(diff); //向きを変更する
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Wall") //特定のTagの付いたオブジェクトを判別
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

        controller.enabled                 = false;        // CharacterControllerを無効に
        this.gameObject.transform.position = Vector3.zero; //プレイヤーのポジションを(0,0,0)に
        controller.enabled                 = true;         // CharacterControllerを有効に
    }
}
