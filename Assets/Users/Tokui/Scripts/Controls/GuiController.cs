using UnityEngine;
using System.Collections;
using UnityEngine.UI;   // uGUI���X�N���v�g�œ����������Ƃ��ɕK�v
using UnityEngine.EventSystems; // uGUI�̃C�x���g�n�C���^�[�t�F�[�X���g�������Ƃ��ɕK�v

public class GuiController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler    // �g�p����C�x���g�n�C���^�[�t�F�[�X(IDragHandler�Ȃ�)�������ɒǉ�����
{
    [SerializeField] Image JOYSTICK_BACK;   // JoyStick_Back�e�N�X�`�������蓖�Ă�B
    [SerializeField] Image JOYSTICK_CENTER; // JoyStick_Center�e�N�X�`�������蓖�Ă�B

    private Vector2 m_StartPosition = Vector2.zero; // �}�E�X�_�E�����W
    private float m_Radius = 0.0f;  // JOYSTICK_BACK�̔��a

    // Use this for initialization
    void Start()
    {
        // �N������͔�\���ɂ���
        JOYSTICK_BACK.enabled = false;
        JOYSTICK_CENTER.enabled = false;

        // JOYSTICK_BACK�̔��a���擾����
        m_Radius = JOYSTICK_BACK.GetComponent<RectTransform>().sizeDelta.x / 2;
    }
    void Update()
    {

    }
    // �}�E�X�_�E���i�C�x���g�n�C���^�[�t�F�[�X�j
    public void OnPointerDown(PointerEventData data)
    {
        // �J�[�\���ʒu��JOYSTICK_BACK��\������
        Vector2 pos = GetLocalPosition(data.position);
        JOYSTICK_BACK.rectTransform.localPosition = pos;
        m_StartPosition = pos;
        JOYSTICK_BACK.enabled = true;
    }

    // �}�E�X�A�b�v�i�C�x���g�n�C���^�[�t�F�[�X�j
    public void OnPointerUp(PointerEventData data)
    {
        // �W���C�X�e�B�b�N���\���ɂ���
        JOYSTICK_BACK.enabled = false;
        JOYSTICK_CENTER.enabled = false;
    }

    // �h���b�O�i�C�x���g�n�C���^�[�t�F�[�X�j
    public void OnDrag(PointerEventData data)
    {
        Vector2 pos = GetLocalPosition(data.position);
        float dx = pos.x - m_StartPosition.x;
        float dy = pos.y - m_StartPosition.y;
        float rad = Mathf.Atan2(dy, dx);
        rad = rad * Mathf.Rad2Deg;

        // JOYSTICK_BACK�̓����Ȃ�΁A�f���Ƀ}�E�X�J�[�\���ʒu��JOYSTICK_CENTER��u��
        if (Vector2.Distance(pos, JOYSTICK_BACK.rectTransform.localPosition) <= m_Radius)
            JOYSTICK_CENTER.rectTransform.localPosition = GetLocalPosition(data.position);
        // JOYSTICK_BACK�̊O���Ȃ�΁AJOYSTICK_BACK�̉~�����JOYSTICK_CENTER��u��
        else
            JOYSTICK_CENTER.rectTransform.localPosition = new Vector2(m_StartPosition.x + m_Radius * Mathf.Cos(rad * Mathf.Deg2Rad),
                                                                        m_StartPosition.y + m_Radius * Mathf.Sin(rad * Mathf.Deg2Rad));
        JOYSTICK_CENTER.enabled = true;
    }

    // Canvas��̍��W���Z�o����("ScreenSpace-Camera"�p)
    private Vector2 GetLocalPosition(Vector2 screenPosition)
    {
        Vector2 result = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(this.GetComponent<RectTransform>(), screenPosition, Camera.main, out result);
        return result;
    }
}