using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallJudge : MonoBehaviour
{
    float GetAngle(Vector2 start, Vector2 target)
    {
        Vector2 dt = target - start;
        float rad = Mathf.Atan2(dt.y, dt.x);
        float degree = rad * Mathf.Rad2Deg;

        return degree;
    }

    void Start()
    {

    }

    void Update()
    {
        
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!collision.collider.CompareTag("Wall")) return;

        // Wall�^�O�𔻕�
        // ����1�Ȃ� return ������
        // collision.contacts ���璆�g�����Ă����Atan �Ŋp�x�����߁A180�x(�}5�x)�͈̔͂��画����m�F����
        // �ڐG�_��3�_�ȏ゠�鎞�Ɋp�x�̐��̑S�ʂ�̔�r���ł���悤�ɂ���
        // ��r���Ă����r���Ńv���C���[�ƐڐG����������������玀�S������Ƃ�A���̎��_�ŏ������I������

        // �z��Ȃ� collision.contacts.length �Ō����m�F�ł���
        // ���X�g�Ȃ� collision.contacts.count �Ō����m�F�ł���

        // Vector3(�O����)��X�AZ�̃f�[�^��Vector2(�񎟌�)��X�AY�ɕϊ�
        Vector2 pos = new Vector2(transform.position.x, transform.position.z);

        // �z�񂩂�Stack�ɕϊ�
        Stack<ContactPoint> s = new Stack<ContactPoint>(collision.contacts);

        Debug.Log(collision.contacts.Length);

        while (s.Count > 0)
        {
            // Stack�������o���ĕϐ��Ɋi�[
            ContactPoint poppedContact = s.Pop();

            // Stack��foreach�ŉ񂵁A���o�������̂Ɣ�r���Ă���
            foreach (ContactPoint contact in s)
            {

                // ���o����contact(poppedContact)�ƍ��A�񂵂Ă�contact��Vector2�ɕϊ�
                // �econtact�̍��W�ƃv���C���[�̍��W����p�x�����߂�
                // �p�x�̓v���C���[(transform.position)����contact�̕����œ��ꂵ�ċ��߂�
                // ��̊p�x�����������������
                // �������l��180��������death

                Vector2 contact1 = ContactPoint(contact);
                Vector2 popContact = ContactPoint(poppedContact);

                float tan = GetAngle(contact1, popContact);
                Debug.Log(tan);

                if (tan == 180)
                {
                    Debug.Log("DEATH");
                    break;
                }

                else if (tan == -180)
                {
                    Debug.Log("DEATH");
                    break;
                }
            }
        }
    }

    private Vector2 ContactPoint(ContactPoint contact)
    {
        return new Vector2(contact.point.x, contact.point.z);
    }

    //foreach (ContactPoint contact in collision.contacts)
    //{
    //  if (collision.gameObject.tag == "Wall")
    //  {
    //    if (collision.contacts.Length == 2)
    //    {
    //       Debug.Log("Death");
    //    }
    //   }
    //}
}

