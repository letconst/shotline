using UnityEngine;

public class TouchController : MonoBehaviour
{

    // Start is called before the first frame update
    public Rigidbody rb;

    public float speed;

    private Animator anim;

    private bool move, rotation;

    private Vector2 startPos, currentPos, differenceDisVector2;

    public static bool Hit = false;

    [SerializeField]
    float radian, differenceDisFloat;

    [SerializeField]
    float PlayerSpeed = 5f;

    [SerializeField]
    float PlayerMinSpeed = 0.25f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        speed = 10;
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        MovementControll();
    }

    void FixedUpdate()
    {
        Movement();
    }

    void MovementControll()
    {
        if (RoundManager.RoundMove == true)
        {
            return;
        }

        #region エディター上での動作
#if UNITY_EDITOR
        //移動
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            //マウス左クリック時に始点座標を代入
            startPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {

            //押している最中に今の座標を代入
            currentPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            differenceDisVector2 = currentPos - startPos;

            //スワイプ量によってSpeedを変化させる.この時、絶対値にする。
            differenceDisFloat = differenceDisVector2.x * differenceDisVector2.y;
            differenceDisFloat /= 100;
            differenceDisFloat = Mathf.Abs(differenceDisFloat);

            //タップしただけで動いてしまうので、距離が短ければ動かないようにする。
            if (differenceDisFloat > 1)
            {
                move = true;


                //最高速度
                if (differenceDisFloat > PlayerSpeed)
                {
                    differenceDisFloat = PlayerSpeed;
                }

                //最低速度
                if (differenceDisFloat < PlayerMinSpeed)
                {
                    differenceDisFloat = PlayerMinSpeed;
                }

                speed = differenceDisFloat;
                //もしspeedが0以上であれば、アニメーションさせる
                if (speed > 0)
                {
                    //anim.SetBool("is_walking", true);
                }

                //回転する角度計算
                radian = Mathf.Atan2(differenceDisVector2.x, differenceDisVector2.y) * Mathf.Rad2Deg;
            }
        }
        else
        {
            rotation = false;
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            speed = 0;
            move = false;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            //anim.SetBool("is_walking", false);
        }
#endif
        #endregion
        
        #region IOS上での動作

#if UNITY_IOS
        //移動
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            //マウス左クリック時に始点座標を代入
            startPos = new Vector2(t.position.x, t.position.y);
        }

        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            //押している最中に今の座標を代入
            currentPos = new Vector2(t.position.x, t.position.y);
            differenceDisVector2 = currentPos - startPos;

            //スワイプ量によってSpeedを変化させる.この時、絶対値にする。
            differenceDisFloat = differenceDisVector2.x * differenceDisVector2.y;
            differenceDisFloat /= 100;
            differenceDisFloat = Mathf.Abs(differenceDisFloat);

            //タップしただけで動いてしまうので、距離が短ければ動かないようにする。
            if (differenceDisFloat > 1)
            {
                move = true;


                //最高速度
                if (differenceDisFloat > PlayerSpeed)
                {
                    differenceDisFloat = PlayerSpeed;
                }

                //最低速度
                if (differenceDisFloat < PlayerMinSpeed)
                {
                    differenceDisFloat = PlayerMinSpeed;
                }

                speed = differenceDisFloat;
                //もしspeedが0以上であれば、アニメーションさせる
                if (speed > 0)
                {
                    //anim.SetBool("is_walking", true);
                }

                //回転する角度計算
                radian = Mathf.Atan2(differenceDisVector2.x, differenceDisVector2.y) * Mathf.Rad2Deg;
            }
        }
        else
        {
            rotation = false;
        }

        if (Input.touchCount >0)
        {
            speed = 0;
            move = false;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            //anim.SetBool("is_walking", false);
        }

#endif
        #endregion
    }
    
    void Movement()
    {
        if (RoundManager.RoundMove == true)
        {
            return;
        }

        if (move == true)
        {
            rb.velocity = transform.forward * speed;
            rb.transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, radian, 0), 10);
        }
    }

    // 要変更
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