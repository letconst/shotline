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

    private Rigidbody _rig;

    public        float CurrentSpeed => _speed * speedRatio;
    public static bool  IsMoving     { get; private set; }

    public void ManagedStart()
    {
        speedRatio = 1;
        _joystick  = GameObject.FindGameObjectWithTag("Joystick").GetComponent<Joystick>();
        // controller = GetComponent<CharacterController>(); //CharacterControllerの取得
        _rig       = GetComponent<Rigidbody>();
    }

    public void ManagedUpdate()
    {
        // IsMoving = false;

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

        _moveX = _joystick.Position.x * CurrentSpeed; //JoystickのPositionに_speedをかけて、_moveXに代入
        _moveZ = _joystick.Position.y * CurrentSpeed; //JoystickのPositionに_speedをかけて、_moveYに代入

        // 2pはカメラを反転させるため、移動方向も逆に
        if (!NetworkManager.IsOwner)
        {
            _moveX = -_moveX;
            _moveZ = -_moveZ;
        }
    }

    private void FixedUpdate()
    {
        if (!MainGameController.isControllable) return;

        IsMoving = false;

        if (_joystick.Position.y > 0.01f || _joystick.Position.y < -0.01f)
        {
            if (_joystick.Position.x > 0.01f || _joystick.Position.x < -0.01f)
            {
                IsMoving = true;

                Vector3 direction = Vector3.zero;
                direction.x = _moveX;
                direction.z = _moveZ;

                // 移動方向にキャラクターが向くようにする
                if (direction != Vector3.zero)
                {
                    transform.localRotation = Quaternion.LookRotation(direction);
                }

                _rig.velocity = direction;
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
