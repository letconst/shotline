using UnityEngine;
using System.Collections;

public class CharaMove : MonoBehaviour, IManagedMethod
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

    CharacterController controller; //CharacterControllerの読み込み

    public float CurrentSpeed => _speed * speedRatio;

    public void ManagedStart()
    {
        speedRatio = 1;
        _joystick  = GameObject.FindGameObjectWithTag("Joystick").GetComponent<Joystick>();
        controller = GetComponent<CharacterController>(); //CharacterControllerの取得
    }

    public void ManagedUpdate()
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

        _moveX = _joystick.Position.x * _speed; //JoystickのPositionに_speedをかけて、_moveXに代入
        _moveZ = _joystick.Position.y * _speed; //JoystickのPositionに_speedをかけて、_moveYに代入

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

        // 移動方向にキャラクターが向くようにする

        if (_joystick.Position.y > 0.01f || _joystick.Position.y < -0.01f)
        {
            if (_joystick.Position.x > 0.01f || _joystick.Position.x < -0.01f)
            {
                Vector3 direction = new Vector3(_moveX, 0, _moveZ);
                transform.localRotation = Quaternion.LookRotation(direction);
                controller.SimpleMove(direction);
            }
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
