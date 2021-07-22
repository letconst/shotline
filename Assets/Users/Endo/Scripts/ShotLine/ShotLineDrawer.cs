using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShotLineDrawer : SingletonMonoBehaviour<ShotLineDrawer>
{
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

    [SerializeField, Header("ラインゲージ")]
    private GameObject Gauge;


    private GameObject _linePrefab;
    private bool       _isHoldClicking; // 射線を描いている最中か
    private Camera     _camera;
    private Vector3    _shotLineCamPos;  // タップ開始時の射線カメラの位置
    private Vector3    _prevLineCamPos;  // 1フレーム前の射線カメラの位置
    private Vector2    _screenCenterPos; // 画面の中心位置
    private int        _currentFingerId; // 現在射線を描いている指ID
    public static float      currentDis;     //最新のゲージ消費量

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
        _currentFingerId = -1;
        _lineDataList    = new List<LineData>();
        DrawingData      = null;
        currentDis = 0;

        // this.UpdateAsObservable()
        //     .Where(_ => _isHoldClicking)
        //     .Subscribe(async _ =>
        //     {
        //         SoundManager.Instance.PlaySE(SELabel.Draw);
        //     });
    }

    private void Start()
    {
        //Gauge.SetActive(false);
        _linePrefab = MainGameController.linePrefab;

        // 射線データ生成
        for (int i = 0; i < initPoolingCount; i++)
        {
            _lineDataList.Add(new LineData(_linePrefab));
        }
    }

    private void Update()
    {

        if (Input.touchCount > 0)
        {
            MobileInputHandler();
        }
        else
        {
            PCInputHandler();
        }
    }

    private void LateUpdate()
    {
        FollowLineToCamera();
    }

    /// <summary>
    /// タッチ入力の処理
    /// </summary>
    private void MobileInputHandler()
    {
        Touch[] touches = Input.touches;

        foreach (Touch touch in touches)
        {
            // 描いている最中、その指IDでなければ弾く
            if (_currentFingerId != -1 && touch.fingerId != _currentFingerId) continue;

            Vector2 touchPos = touch.position;

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    // 画面中央からのドローのみ受け付ける
                    if (Vector2.Distance(_screenCenterPos, touchPos) > drawableAreaRadius) return;

                    // 移動中は反応させない（暫定）
                    if (CharaMove.IsMoving) return;

                    // UIをクリックした際は反応させない
                    if (EventSystem.current.IsPointerOverGameObject(touch.fingerId)) return;

                    if (_currentFingerId == -1)
                    {
                        _currentFingerId = touch.fingerId;
                    }

                    //ラインゲージの引き直し分をゲージにプラス、初回は0
                    LineGaugeController.Instance.preslider.fillAmount += currentDis;
                    currentDis = 0;
                    LineGaugeController.AbleDraw = true;

                    _isHoldClicking = true;
                    _shotLineCamPos = lineCamera.transform.position;

                    CreateLine(touchPos);

                    break;

                case TouchPhase.Moved when _isHoldClicking:
                    // 射線が固定されていたら処理しない（描きながら射撃した際にも止める）
                    // 移動中は処理しない（暫定）
                    if (DrawingData.IsFixed || CharaMove.IsMoving)
                    {
                        _isHoldClicking  = false;
                        _currentFingerId = -1;

                        break;
                    }

                    List<Vector3> drawingFingerPos = DrawingData.FingerPositions;
                    Vector3       tmpTouchPos      = touchPos;
                    tmpTouchPos.z += lineZPos;
                    Vector3 worldFingerPos = _camera.ScreenToWorldPoint(tmpTouchPos);

                    if (Vector2.Distance(worldFingerPos, drawingFingerPos[drawingFingerPos.Count - 1]) > lineFineness)
                    {
                        UpdateLine(touchPos);
                    }

                    break;

                case TouchPhase.Ended:
                    _isHoldClicking = false;
                    _currentFingerId = -1;

                    break;
            }
        }
    }

    /// <summary>
    /// マウス入力の処理
    /// </summary>
    private void PCInputHandler()
    {
        Vector3 mousePos = Input.mousePosition;

        if (Input.GetMouseButtonDown(0))
        {
            // UIをクリックした際は反応させない
            if (EventSystem.current.IsPointerOverGameObject()) return;

            // 画面中央からのドローのみ受け付ける
            if (Vector2.Distance(_screenCenterPos, mousePos) > drawableAreaRadius) return;

            //ラインゲージの引き直し分をゲージにプラス、初回は0
            LineGaugeController.Instance.preslider.fillAmount += currentDis;
            currentDis = 0;
            LineGaugeController.AbleDraw = true;


            _isHoldClicking = true;
            _shotLineCamPos = lineCamera.transform.position;

            CreateLine(mousePos);
        }
        else if (Input.GetMouseButton(0) && _isHoldClicking)
        {
            // 1つ前の射線位置から指定距離離れていたら伸ばす
            List<Vector3> drawingFingerPos = DrawingData.FingerPositions;
            Vector3       tmpMousePos      = mousePos;
            tmpMousePos.z += lineZPos;
            Vector3 worldMousePos = _camera.ScreenToWorldPoint(tmpMousePos);

            if (Vector2.Distance(worldMousePos, drawingFingerPos[drawingFingerPos.Count - 1]) > lineFineness)
            {
                UpdateLine(mousePos);
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _isHoldClicking = false;
        }
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
        var deltaPosToCam = new Vector3(curLineCamPos.x - _shotLineCamPos.x,
                                        curLineCamPos.y - _shotLineCamPos.y,
                                        curLineCamPos.z - _shotLineCamPos.z);

        List<Vector3> drawingFingerPos     = DrawingData.FingerPositions;
        int           fingerPositionsCount = drawingFingerPos.Count;

        // 射線の全ポイントの座標を更新
        for (int i = 0; i < fingerPositionsCount; i++)
        {
            var newPos = new Vector3(drawingFingerPos[i].x + deltaPosToCam.x,
                                     drawingFingerPos[i].y + deltaPosToCam.y,
                                     drawingFingerPos[i].z + deltaPosToCam.z);

            DrawingData.Renderer.SetPosition(i, newPos);
        }

        _prevLineCamPos = curLineCamPos;
    }

    /// <summary>
    /// 射線を作成する
    /// </summary>
    private void CreateLine(Vector3 newFingerPosition)
    {
        //ゲージを表示
        Gauge.SetActive(true);

        if (LineGaugeController.AbleDraw)
        {

            SoundManager.Instance.PlaySE(SELabel.Draw);

            newFingerPosition.z += lineZPos;
            Vector3 worldFingerPos = _camera.ScreenToWorldPoint(newFingerPosition);

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
                targetData = InstantiateNewLineData();
                DrawingData = targetData;
            }
            else
            {
                DrawingData = targetData;
                ShotLineUtil.ClearLineData(DrawingData);
            }

            // タップ位置に起点を設定
            List<Vector3> targetDataFingerPositions = DrawingData.FingerPositions;
            targetDataFingerPositions.Clear();
            targetDataFingerPositions.Add(worldFingerPos);
            targetDataFingerPositions.Add(worldFingerPos);
            DrawingData.Renderer.SetPosition(0, worldFingerPos);
            DrawingData.Renderer.SetPosition(1, worldFingerPos);
            DrawingData.Renderer.enabled = true;
            DrawingData.IsFixed = false;

        }
    }

    /// <summary>
    /// ドロー中の射線を伸ばす
    /// </summary>
    /// <param name="newFingerPos">追加する位置</param>
    private void UpdateLine(Vector3 newFingerPos)
    {
        float rdis = 0;

        // TODO: 射線長の上限
        if (LineGaugeController.AbleDraw)
        {
            if (DrawingData == null)
            {
                Debug.LogError("ドロー中の射線データがありません");

                return;
            }

            newFingerPos.z += lineZPos;
            Vector3 worldFingerPos = _camera.ScreenToWorldPoint(newFingerPos);

            float dis = Vector3.Distance(DrawingData.FingerPositions[DrawingData.FingerPositions.Count-1],worldFingerPos);
            bool isDraw = LineGaugeController.LineGauge(dis, ref rdis);
            currentDis += dis/LineGaugeController.Instance.MaxLinePower;
            LineGaugeController.holdAmount = LineGaugeController.Instance.preslider.fillAmount;

            if (!isDraw)
            {
                worldFingerPos = Vector3.Lerp(DrawingData.FingerPositions[DrawingData.FingerPositions.Count - 1], worldFingerPos, rdis / dis);
            }

            DrawingData.FingerPositions.Add(worldFingerPos);
            DrawingData.Renderer.positionCount++;
            DrawingData.Renderer.SetPosition(DrawingData.Renderer.positionCount - 1, worldFingerPos);

        }
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

    /// <summary>
    /// 射線データを新たに生成し、管理下に加える
    /// </summary>
    /// <returns>生成された射線データ</returns>
    private static LineData InstantiateNewLineData()
    {
        LineData newData = ShotLineUtil.InstantiateLine(Instance._linePrefab);
        Instance._lineDataList.Add(newData);

        return newData;
    }

    /// <summary>
    /// 使用されていない射線データを取得する
    /// </summary>
    /// <returns>射線データ</returns>
    public static LineData GetFreeLine()
    {
        foreach (LineData lineData in Instance._lineDataList)
        {
            if (lineData.IsFixed) continue;

            return lineData;
        }

        // すべて使用済みなら新たに生成
        return InstantiateNewLineData();
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
