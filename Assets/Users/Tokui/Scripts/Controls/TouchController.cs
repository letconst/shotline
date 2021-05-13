using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchController : MonoBehaviour
{

    // Start is called before the first frame update
    public Rigidbody rb;
    public float speed;
    private Animator anim;
    private bool move, rotation;
    private Vector2 startPos, currentPos, differenceDisVector2;
    [SerializeField] private float radian, differenceDisFloat;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        speed = 0;
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
        //�ړ�
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            //�}�E�X���N���b�N���Ɏn�_���W����
            startPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {

            //�����Ă���Œ��ɍ��̍��W����
            currentPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            differenceDisVector2 = currentPos - startPos;

            //�X���C�v�ʂɂ����Speed��ω�������.���̎��A��Βl�ɂ���B
            differenceDisFloat = differenceDisVector2.x * differenceDisVector2.y;
            differenceDisFloat /= 100;
            differenceDisFloat = Mathf.Abs(differenceDisFloat);

            //�^�b�v���������œ����Ă��܂��̂ŁA�������Z����Γ����Ȃ��悤�ɂ���B
            if (differenceDisFloat > 1)
            {
                move = true;


                //�ō����x
                if (differenceDisFloat > 1.25f)
                {
                    differenceDisFloat = 1.25f;
                }

                //�Œᑬ�x
                if (differenceDisFloat < 0.25f)
                {
                    differenceDisFloat = 0.25f;
                }

                speed = differenceDisFloat;
                //����speed��0�ȏ�ł���΁A�A�j���[�V����������
                if (speed > 0)
                {
                    anim.SetBool("is_walking", true);
                }

                //��]����p�x�v�Z
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
            anim.SetBool("is_walking", false);
        }
    }

    void Movement()
    {
        if (move == true)
        {
            rb.velocity = transform.forward * speed;
            rb.transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, radian, 0), 10);
        }
    }

}