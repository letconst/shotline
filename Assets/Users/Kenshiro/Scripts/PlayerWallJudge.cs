using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallJudge : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        // Wall�^�O�𔻕�
        // ����1�Ȃ� return ������
        // collision.contacts ���璆�g�����Ă����Atan �Ŋp�x�����߁A180�x(�}5�x)�͈̔͂��画����m�F����
        // �ڐG�_��3�_�ȏ゠�鎞�Ɋp�x�̐��̑S�ʂ�̔�r���ł���悤�ɂ���
        // ��r���Ă����r���Ńv���C���[�ƐڐG����������������玀�S������Ƃ�A���̎��_�ŏ������I������

        // �z��Ȃ� collision.contacts.length �Ō����m�F�ł���
        // ���X�g�Ȃ� collision.contacts.count �Ō����m�F�ł���

        if (collision.gameObject.tag == "Wall")
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                if (collision.contacts.Length == 1)
                {
                    Debug.Log("aa");
                    return;
                }

                else if(collision.contacts.Length == 2)
                {
                    Debug.Log("Death");
                }
            }
        }    
    }
}
