using UnityEngine;

/// <summary>
/// ジョイスティック
/// </summary>
public class Joystick : MonoBehaviour
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

    private Vector2 _beganTouchpos;

    private bool _isClicking;

    //スティックの位置(Setter)
    private Vector3 _stickPosition
    {
        set
        {
            _stick.transform.localPosition = value;
            _position = new Vector3(
              _stick.transform.localPosition.x / _radius,
              _stick.transform.localPosition.y / _radius
            );
        }
    }
    
    //=================================================================================
    //ドラッグ
    //=================================================================================

    //ドラッグ中
    public void Update()
    {
        if (Input.touchCount > 0)
        {
            //タップ位置を画面内の座標に変換し、スティックを移動
            foreach (Touch touch in Input.touches)
            {
                if (touch.fingerId != _fingerID && _fingerID != -1)
                {
                    continue;
                }

                if (touch.phase == TouchPhase.Began)
                {
                    float currentRadius = Vector3.Distance(touch.position, _stick.transform.position);

                    if (currentRadius > _radius)
                    {
                        continue;
                    }

                    if (touch.fingerId != _fingerID && _fingerID == -1)
                    {
                        _fingerID = touch.fingerId;
                        _beganTouchpos = touch.position;
                    }
                }

                if (touch.phase == TouchPhase.Moved && _fingerID != -1)
                {
                    Vector3 screenPos = touch.position - _beganTouchpos;
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

                if (touch.phase == TouchPhase.Ended)
                {
                    _fingerID = -1;
                    if (_shouldResetPosition)
                    {
                        //スティックを中心に戻す
                        _stickPosition = Vector3.zero;
                    }
                }
            }
        }

        #region //Unityエディター上での動作確認用
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                float currentRadius = Vector3.Distance(Input.mousePosition, _stick.transform.position);

                if (currentRadius < _radius)
                {
                    _isClicking = true;
                }
            }
            else if(Input.GetMouseButton(0) && _isClicking)
            {
                Vector2 screenPos = Vector2.zero;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(),
                  new Vector2(Input.mousePosition.x, Input.mousePosition.y),
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
            else if(Input.GetMouseButtonUp(0))
            {
                if (_shouldResetPosition)
                {
                    //スティックを中心に戻す
                    _stickPosition = Vector3.zero;
                }

                _isClicking = false;
            }
        }
        #endregion

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
