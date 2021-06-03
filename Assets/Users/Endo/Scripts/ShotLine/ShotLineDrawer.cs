using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShotLineDrawer : SingletonMonoBehaviour<ShotLineDrawer>
{
    [SerializeField, Header("射線オブジェクト")]
    private GameObject linePrefab;

    [SerializeField, Header("射線のZ位置")]
    private float lineZPos = 9.8f;

    [SerializeField, Header("射線の太さ"), Range(.1f, 1)]
    private float lineFineness = .1f;

    [SerializeField, Header("射線を引ける領域の半径")]
    private float drawableAreaRadius;

    [SerializeField, Header("射線のカメラ")]
    private Camera lineCamera;

    [SerializeField, Header("ゲーム開始時、事前に生成しておく射線オブジェクトの数")]
    private int initPoolingCount = 5;

    private bool    _isHoldClicking; // 射線を描いている最中か
    private Camera  _camera;
    private Vector3 _shotLineCamPos;  // タップ開始時の射線カメラの位置
    private Vector3 _prevLineCamPos;  // 1フレーム前の射線カメラの位置
    private Vector2 _screenCenterPos; // 画面の中心位置

    private List<LineData> _lineDataList;

    /// <summary>
    /// ドロー中の射線データ。ドロー開始時から射撃が終了するまで保持される。
    /// </summary>
    public static LineData DrawingData { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        _camera          = Camera.main;
        _shotLineCamPos  = Vector3.zero;
        _prevLineCamPos  = Vector3.zero;
        _screenCenterPos = new Vector2(Screen.width / 2f, Screen.height / 2f);
        _lineDataList    = new List<LineData>();

        // 射線データ生成
        for (int i = 0; i < initPoolingCount; i++)
        {
            _lineDataList.Add(new LineData(linePrefab));
        }
    }

    private void Update()
    {
        DrawLine();
    }

    private void LateUpdate()
    {
        FollowLineToCamera();
    }

    /// <summary>
    /// 入力によって射線を描画する
    /// </summary>
    private void DrawLine()
    {
#if UNITY_EDITOR
        Vector3 mousePos = Input.mousePosition;
        mousePos.z += lineZPos;
        Vector3 tmpMousePos = _camera.ScreenToWorldPoint(mousePos);

        if (Input.GetMouseButtonDown(0))
        {
            // UIをクリックした際は反応させない
            if (EventSystem.current.IsPointerOverGameObject()) return;

            // 画面中央からのドローのみ受け付ける
            if (Vector2.Distance(_screenCenterPos, mousePos) > drawableAreaRadius) return;

            _isHoldClicking = true;
            _shotLineCamPos = lineCamera.transform.position;

            CreateLine();
        }
        else if (Input.GetMouseButton(0) && _isHoldClicking)
        {
            // 1つ前の射線位置から指定距離離れていたら伸ばす
            List<Vector3> drawingFingerPos = DrawingData.FingerPositions;

            if (Vector2.Distance(tmpMousePos, drawingFingerPos[drawingFingerPos.Count - 1]) > lineFineness)
            {
                UpdateLine(tmpMousePos);
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _isHoldClicking = false;
        }
#elif UNITY_IOS || UNITY_ANDROID
        if (Input.touchCount > 0)
        {
            Touch   t = Input.GetTouch(0);
            Vector3 touchPos = t.position;
            touchPos.z += lineZPos;
            Vector3 tmpFingerPos = _camera.ScreenToWorldPoint(touchPos);

            switch (t.phase)
            {
                case TouchPhase.Began:
                    if (EventSystem.current.IsPointerOverGameObject()) return;

                    if (Vector2.Distance(_screenCenterPos, touchPos) > drawableAreaRadius) return;

                    _isHoldClicking = true;
                    _shotLineCamPos = lineCamera.transform.position;

                    CreateLine();

                    break;

                case TouchPhase.Moved when _isHoldClicking:
                    List<Vector3> drawingFingerPos = DrawingData.FingerPositions;

                    if (Vector2.Distance(tmpFingerPos, drawingFingerPos[drawingFingerPos.Count - 1]) > lineFineness)
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
        if (DrawingData == null) return;

        // 射線が描画状態であり、かつ固定されていないときのみ処理
        if (!DrawingData.Renderer.enabled || DrawingData.IsFixed) return;

        Vector3 curLineCamPos = lineCamera.transform.position;

        // カメラの位置が1フレーム前と同じなら処理しない
        if (curLineCamPos == _prevLineCamPos) return;

        // 射線のドロー開始時と現在の射線カメラの差分座標
        Vector3 deltaPosToCam = new Vector3(curLineCamPos.x - _shotLineCamPos.x,
                                            curLineCamPos.y - _shotLineCamPos.y,
                                            curLineCamPos.z - _shotLineCamPos.z);

        List<Vector3> drawingFingerPos     = DrawingData.FingerPositions;
        int           fingerPositionsCount = drawingFingerPos.Count;

        // 射線の全ポイントの座標を更新
        for (int i = 0; i < fingerPositionsCount; i++)
        {
            Vector3 newPos = new Vector3(drawingFingerPos[i].x + deltaPosToCam.x,
                                         drawingFingerPos[i].y + deltaPosToCam.y,
                                         drawingFingerPos[i].z + deltaPosToCam.z);

            DrawingData.Renderer.SetPosition(i, newPos);
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
#elif UNITY_IOS || UNITY_ANDROID
        touchPos = Input.GetTouch(0).position;
#endif

        touchPos.z += lineZPos;
        Vector3 worldTouchPos = _camera.ScreenToWorldPoint(touchPos);

        LineData targetData = null;

        // 未使用の射線があるか確認
        foreach (LineData data in _lineDataList)
        {
            if (data.IsFixed) continue;

            targetData = data;

            break;
        }

        // なければ生成
        if (targetData == null)
        {
            LineData newData = ShotLineUtil.InstantiateLine(linePrefab);
            _lineDataList.Add(newData);
            targetData  = newData;
            DrawingData = targetData;
        }
        else
        {
            DrawingData = targetData;
            ShotLineUtil.ClearLineData(DrawingData);
        }

        // タップ位置に起点を設定
        List<Vector3> targetDataFingerPositions = targetData.FingerPositions;
        targetDataFingerPositions.Clear();
        targetDataFingerPositions.Add(worldTouchPos);
        targetDataFingerPositions.Add(worldTouchPos);
        targetData.Renderer.SetPosition(0, targetDataFingerPositions[0]);
        targetData.Renderer.SetPosition(1, targetDataFingerPositions[1]);
        targetData.Renderer.enabled = true;
        targetData.IsFixed          = false;
    }

    /// <summary>
    /// ドロー中の射線を伸ばす
    /// </summary>
    /// <param name="newFingerPos">追加する位置</param>
    private void UpdateLine(Vector3 newFingerPos)
    {
        // TODO: 射線長の上限

        if (DrawingData == null)
        {
            Debug.LogError("ドロー中の射線データがありません");

            return;
        }

        DrawingData.FingerPositions.Add(newFingerPos);
        DrawingData.Renderer.positionCount++;
        DrawingData.Renderer.SetPosition(DrawingData.Renderer.positionCount - 1, newFingerPos);
    }

    /// <summary>
    /// すべての射線の描画をクリアする
    /// </summary>
    public static void ClearAllLine()
    {
        foreach (LineData data in Instance._lineDataList)
        {
            if (data.IsFixed) continue;

            ShotLineUtil.ClearLineData(data);
        }
    }
}

public class LineData
{
    public readonly GameObject    LineObj;
    public readonly LineRenderer  Renderer;
    public          bool          IsFixed;
    public readonly List<Vector3> FingerPositions;

    public LineData(GameObject linePrefab)
    {
        LineObj          = Object.Instantiate(linePrefab);
        Renderer         = LineObj.GetComponent<LineRenderer>();
        Renderer.enabled = false; // 初回非表示
        IsFixed          = false;
        FingerPositions  = new List<Vector3>();
    }
}
