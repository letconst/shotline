using UnityEngine;
using UnityEngine.UI;   // uGUIをスクリプトで動かしたいときに必要
using UnityEngine.EventSystems; // uGUIのイベント系インターフェースを使いたいときに必要

public class GuiController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler    // 使用するイベント系インターフェース(IDragHandlerなど)をここに追加する
{
    [SerializeField] Image JOYSTICK_BACK;   // JoyStick_Backテクスチャを割り当てる。
    [SerializeField] Image JOYSTICK_CENTER; // JoyStick_Centerテクスチャを割り当てる。

    private Vector2 m_StartPosition = Vector2.zero; // マウスダウン座標
    private float m_Radius = 0.0f;  // JOYSTICK_BACKの半径

    // Use this for initialization
    void Start()
    {
        // 起動直後は非表示にする
        JOYSTICK_BACK.enabled = false;
        JOYSTICK_CENTER.enabled = false;

        // JOYSTICK_BACKの半径を取得する
        m_Radius = JOYSTICK_BACK.GetComponent<RectTransform>().sizeDelta.x / 2;
    }
    void Update()
    {
        
    }


    #region マウス操作に応じたジョイスティックの動作
    // マウスダウン
    public void OnPointerDown(PointerEventData data)
    {
        // カーソル位置にJOYSTICK_BACKを表示する
        Vector2 pos = GetLocalPosition(data.position);
        JOYSTICK_BACK.rectTransform.localPosition = pos;
        m_StartPosition = pos;
        JOYSTICK_BACK.enabled = true;
    }

    // マウスアップ
    public void OnPointerUp(PointerEventData data)
    {
        // ジョイスティックを非表示にする
        JOYSTICK_BACK.enabled = false;
        JOYSTICK_CENTER.enabled = false;
    }

    // ドラッグ
    public void OnDrag(PointerEventData data)
    {
        Vector2 pos = GetLocalPosition(data.position);
        float dx = pos.x - m_StartPosition.x;
        float dy = pos.y - m_StartPosition.y;
        float rad = Mathf.Atan2(dy, dx);
        rad = rad * Mathf.Rad2Deg;

        // JOYSTICK_BACKの内側ならば、素直にマウスカーソル位置にJOYSTICK_CENTERを置く
        if (Vector2.Distance(pos, JOYSTICK_BACK.rectTransform.localPosition) <= m_Radius)
        {
            JOYSTICK_CENTER.rectTransform.localPosition = GetLocalPosition(data.position);
        }
        // JOYSTICK_BACKの外側ならば、JOYSTICK_BACKの円周上にJOYSTICK_CENTERを置く
        else
        {
            JOYSTICK_CENTER.rectTransform.localPosition = new Vector2(m_StartPosition.x + m_Radius * Mathf.Cos(rad * Mathf.Deg2Rad),
                                                                       m_StartPosition.y + m_Radius * Mathf.Sin(rad * Mathf.Deg2Rad));
        }
        JOYSTICK_CENTER.enabled = true;
    }
    #endregion

    // Canvas上の座標を算出する("ScreenSpace-Camera"用)
    private Vector2 GetLocalPosition(Vector2 screenPosition)
    {
        Vector2 result = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(this.GetComponent<RectTransform>(), screenPosition, Camera.main, out result);
        return result;
    }
}