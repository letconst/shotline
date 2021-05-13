using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShotLineDrawer : SingletonMonoBehaviour<ShotLineDrawer>
{
    [SerializeField, Header("射線オブジェクト")]
    private GameObject linePrefab;

    [SerializeField, Header("射線のZ位置")]
    private float lineZPos = 9.8f;

    [SerializeField, Header("射線の細かさ"), Range(.1f, 1)]
    private float lineFineness = .1f;

    [SerializeField, Header("射線のカメラ")]
    private Camera lineCamera;

    private bool          _isHoldClicking; // 射線を描いている最中か
    private bool          _isFixed;        // 射線が固定されているか
    private Camera        _camera;
    private GameObject    _shotLine;        // 射線オブジェクト
    private Vector3       _shotLineCamPos;  // タップ開始時の射線カメラの位置
    private Vector3       _prevLineCamPos;  // 1フレーム前の射線カメラの位置
    private List<Vector3> _fingerPositions; // 描画された射線の通過位置
    private LineRenderer  _lineRenderer;

    private void Start()
    {
        _camera          = Camera.main;
        _shotLineCamPos  = Vector3.zero;
        _prevLineCamPos  = Vector3.zero;
        _fingerPositions = new List<Vector3>();

        // 射線オブジェクト生成
        if (!_shotLine)
        {
            _shotLine             = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
            _lineRenderer         = _shotLine.GetComponent<LineRenderer>();
            _lineRenderer.enabled = false; // 初回時非表示
        }
    }

    private void Update()
    {
        // TODO: 開始地点をプレイヤー（画面中央）からに限定する

        DrawLine();
        FollowLineToCamera();
    }

    /// <summary>
    /// 入力によって射線を描画する
    /// </summary>
    private void DrawLine()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            // UIをクリックした際は反応させない
            if (EventSystem.current.IsPointerOverGameObject()) return;

            _isHoldClicking = true;
            _shotLineCamPos = lineCamera.transform.position;

            CreateLine();
        }
        else if (Input.GetMouseButton(0) && _isHoldClicking)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z += lineZPos;
            Vector3 tmpFingerPos = _camera.ScreenToWorldPoint(mousePos);

            // 1つ前の射線位置から指定距離離れていたら伸ばす
            if (Vector2.Distance(tmpFingerPos, _fingerPositions[_fingerPositions.Count - 1]) > lineFineness)
            {
                UpdateLine(tmpFingerPos);
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _isHoldClicking = false;
        }
#elif UNITY_IOS
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);

            switch (t.phase)
            {
                case TouchPhase.Began:
                    if (EventSystem.current.IsPointerOverGameObject()) return;

                    _isHoldClicking = true;
                    _shotLineCamPos = lineCamera.transform.position;

                    CreateLine();

                    break;

                case TouchPhase.Moved when _isHoldClicking:
                    Vector3 touchPos = t.position;
                    touchPos.z += lineZPos;
                    Vector3 tmpFingerPos = _camera.ScreenToWorldPoint(touchPos);

                    if (Vector2.Distance(tmpFingerPos, _fingerPositions[_fingerPositions.Count - 1]) > lineFineness)
                    {
                        UpdateLine(tmpFingerPos);
                    }

                    break;

                case TouchPhase.Ended:
                    _isHoldClicking = false;

                    break;
            }
        }
#endif
    }

    /// <summary>
    /// 描画されている射線の位置をカメラに追従させる
    /// </summary>
    private void FollowLineToCamera()
    {
        // 射線が描画状態であり、かつ固定されていないときのみ処理
        if (!_lineRenderer.enabled || _isFixed) return;

        Vector3 curLineCamPos = lineCamera.transform.position;

        // カメラの位置が1フレーム前と同じなら処理しない
        if (curLineCamPos == _prevLineCamPos) return;

        // 射線のドロー開始時と現在の射線カメラの差分座標
        Vector3 deltaPosToCam = new Vector3(curLineCamPos.x - _shotLineCamPos.x,
                                            curLineCamPos.y - _shotLineCamPos.y,
                                            curLineCamPos.z - _shotLineCamPos.z);

        // 射線の全ポイントの座標を更新
        for (int i = 0; i < _fingerPositions.Count; i++)
        {
            Vector3 newPos = new Vector3(_fingerPositions[i].x + deltaPosToCam.x,
                                         _fingerPositions[i].y + deltaPosToCam.y,
                                         _fingerPositions[i].z + deltaPosToCam.z);

            _lineRenderer.SetPosition(i, newPos);
        }

        _prevLineCamPos = curLineCamPos;
    }

    /// <summary>
    /// 射線を作成する
    /// </summary>
    private void CreateLine()
    {
        Vector3 touchPos;

#if UNITY_EDITOR
        touchPos = Input.mousePosition;
#elif UNITY_IOS
        touchPos = Input.GetTouch(0).position;
#endif

        touchPos.z += lineZPos;
        Vector3 worldTouchPos = _camera.ScreenToWorldPoint(touchPos);
        ClearLine();

        // タップ位置に起点を設定
        _fingerPositions.Clear();
        _fingerPositions.Add(worldTouchPos);
        _fingerPositions.Add(worldTouchPos);
        _lineRenderer.SetPosition(0, _fingerPositions[0]);
        _lineRenderer.SetPosition(1, _fingerPositions[1]);
        _lineRenderer.enabled = true;
        _isFixed              = false;
    }

    /// <summary>
    /// 射線レンダラーに追記し、射線を伸ばす
    /// </summary>
    /// <param name="newFingerPos">追加する位置</param>
    private void UpdateLine(Vector3 newFingerPos)
    {
        // TODO: 射線長の上限

        _fingerPositions.Add(newFingerPos);
        _lineRenderer.positionCount++;
        _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, newFingerPos);
    }

    /// <summary>
    /// 射線の位置を固定する
    /// </summary>
    public static void FixLine()
    {
        Instance._isFixed = true;
    }

    /// <summary>
    /// 射線の描画をクリアする
    /// </summary>
    public static void ClearLine()
    {
        Instance._lineRenderer.positionCount = 2;
        Instance._lineRenderer.enabled       = false;
    }

    /// <summary>
    /// 射線の通過座標を取得する
    /// </summary>
    /// <returns>射線の通過座標</returns>
    public static Vector3[] GetFingerPositions()
    {
        Vector3[] fingerPositions = new Vector3[Instance._lineRenderer.positionCount];
        Instance._lineRenderer.GetPositions(fingerPositions);

        return fingerPositions;
    }
}
