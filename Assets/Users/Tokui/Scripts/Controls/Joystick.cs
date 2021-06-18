using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// ジョイスティック
/// </summary>
public class Joystick : Graphic, IPointerDownHandler, IPointerUpHandler, IEndDragHandler, IDragHandler
{

    //実際に動くスティック部分
    [SerializeField]
    [Header("実際に動くスティック部分(自動設定)")]
    private GameObject _stick = null;
    private const string STICK_NAME = "Stick";

    //スティックが動く範囲の半径
    [SerializeField]
    [Header("スティックが動く範囲の半径")]
    private float _radius = 100;

    //指を離した時にスティックが中心に戻るか
    [SerializeField]
    [Header("指を離した時にスティックが中心に戻るか")]
    private bool _shouldResetPosition = true;

    //現在地(x,y共に値が-1~1の範囲になる)
    [SerializeField]
    [Header("現在地(自動更新)")]
    private Vector2 _position = Vector2.zero;
    public Vector2 Position { get { return _position; } }

    private int _fingerID = -1;

    //スティックの位置(Setter)
    private Vector3 _stickPosition
    {
        set
        {
            _stick.transform.localPosition = value;
            _position = new Vector2(
              _stick.transform.localPosition.x / _radius,
              _stick.transform.localPosition.y / _radius
            );
        }
    }

    //=================================================================================
    //初期化
    //=================================================================================

    protected override void Awake()
    {
        base.Awake();
        Init();
    }

    //スクリプトがロードされた時やインスペクターの値が変更されたときに呼び出される(エディター上のみ)
    /*protected override void OnValidate()
    {
        base.OnValidate();
        Init();
    }*/

    //初期化
    private void Init()
    {
        //スティックを生成する必要があれば生成し、位置を中心に設定
        CreateStickIfneeded();
        _stickPosition = Vector3.zero;

        //スティックのImage取得(なければ追加)、タッチ判定を取られないようにraycastTargetをfalseに
        Image stickImage = _stick.GetComponent<Image>();
        if (stickImage == null)
        {
            stickImage = _stick.AddComponent<Image>();
            //stickImage.sprite = Resources.Load<Image>("UI/JoyStick_Center").sprite;
        }
        stickImage.raycastTarget = false;

        //タッチ判定を受け取れるようにRaycastTargetをTrueに
        raycastTarget = true;

        //タッチ判定をとる範囲は表示されないように透明に
        color = new Color(0, 0, 0, 0);
    }

    //スティックを生成する必要があれば生成
    private void CreateStickIfneeded()
    {
        //スティックが設定されていれば終了
        if (_stick != null)
        {
            return;
        }

        //スティックが子にあるか検索、あれば取得し終了
        if (transform.Find(STICK_NAME) != null)
        {
            _stick = transform.Find(STICK_NAME).gameObject;
            return;
        }

        //スティック生成
        _stick = new GameObject(STICK_NAME);
        _stick.transform.SetParent(gameObject.transform);
        _stick.transform.localRotation = Quaternion.identity;
    }

    //=================================================================================
    //タップ
    //=================================================================================

    //タップ開始時
    public void OnPointerDown(PointerEventData eventData)
    {
        //タップした瞬間にドラッグを開始した事にする
        OnDrag(eventData);
    }

    //タップ終了時(ドラッグ終了時には呼ばれない)
    public void OnPointerUp(PointerEventData eventData)
    {
        //タップした終了した時にドラッグを終了した時と同じ処理をする
        OnEndDrag(eventData);
    }

    //=================================================================================
    //ドラッグ
    //=================================================================================

    //ドラッグ終了時
    public void OnEndDrag(PointerEventData eventData)
    {
        if (_shouldResetPosition)
        {
            //スティックを中心に戻す
            _stickPosition = Vector3.zero;
        }
    }

    //ドラッグ中
    public void OnDrag(PointerEventData eventData)
    {
        Touch mytouch = Input.GetTouch(0);

        //Touch[] myTouches = Input.touches;

        //タップ位置を画面内の座標に変換し、スティックを移動
        foreach (Touch touch in Input.touches)
        {
            if (mytouch.fingerId != _fingerID && _fingerID == -1)
            {
                _fingerID = mytouch.fingerId;
            }
            if (mytouch.phase != TouchPhase.Began && mytouch.fingerId != _fingerID)
            {
                continue;
            }

            #region デバッグ用
            switch (mytouch.phase)
            {
                case TouchPhase.Began:
                    Debug.LogFormat("{0}:いまタッチした", _fingerID);
                    break;

                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    Debug.LogFormat("{0}:タッチしている", _fingerID);
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    Debug.LogFormat("{0}:いま離された", _fingerID);
                    break;
            }
            #endregion

            Vector2 screenPos = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(),
              new Vector2(mytouch.position.x, mytouch.position.y),
              null,
              out screenPos
            );
            _stickPosition = screenPos;

            //移動場所が設定した半径を超えてる場合は制限内に抑える
            float currentRadius = Vector3.Distance(Vector3.zero, _stick.transform.localPosition);
            if (currentRadius > _radius)
            {
                //角度計算
                float radian = Mathf.Atan2(_stick.transform.localPosition.y, _stick.transform.localPosition.x);

                //円上にXとYを設定
                Vector3 limitedPosition = Vector3.zero;
                limitedPosition.x = _radius * Mathf.Cos(radian);
                limitedPosition.y = _radius * Mathf.Sin(radian);

                _stickPosition = limitedPosition;
            }
        }

    }

    //=================================================================================
    //更新
    //=================================================================================

#if UNITY_EDITOR
    //Gizmoを表示する
    private void OnDrawGizmos()
    {
        //スティックが移動できる範囲をScene上に表示
        UnityEditor.Handles.color = Color.green;
        UnityEditor.Handles.DrawWireDisc(transform.position, transform.forward, _radius * 0.5f);
    }
#endif

}
