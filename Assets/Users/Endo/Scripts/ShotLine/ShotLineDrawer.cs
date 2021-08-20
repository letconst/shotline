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

    [SerializeField, Header("自分の位置")]
    private GameObject centerPos;


    private GameObject _linePrefab;
    private bool       _isHoldClicking; // 射線を描いている最中か
    private Camera     _camera;
    private Vector3    _shotLineCamPos;  // タップ開始時の射線カメラの位置
    private Vector3    _prevLineCamPos;  // 1フレーム前の射線カメラの位置
    private Vector2    _screenCenterPos; // 画面の中心位置
    private int        _currentFingerId; // 現在射線を描いている指ID
    public static float      currentDis;     //最新のゲージ消費量
    public static bool _firstLinearDraw;   //二回目以降のリニアドローか

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
        _firstLinearDraw = true;
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

            Vector3 touchPos = touch.position;
            touchPos.z += lineZPos;
            Vector3 tmpFingerPos = _camera.ScreenToWorldPoint(touchPos);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    // UIをクリックした際は反応させない
                    if (EventSystem.current.IsPointerOverGameObject(touch.fingerId)) return;


                    if (LinearDraw._isLinearDraw)
                    {
                        //リニアドローオンの場合
                        _isHoldClicking = false;

                        if (!_firstLinearDraw)
                        {
                            UpdateLine(tmpFingerPos);
                        }

                    }
                    else
                    {
                        // 画面中央からのドローのみ受け付ける
                        if (Vector2.Distance(_screenCenterPos, touchPos) > drawableAreaRadius) return;

                        //リニアドローオフの場合
                        _isHoldClicking = true;
                    }

                    if (_currentFingerId == -1)
                    {
                        _currentFingerId = touch.fingerId;
                    }

                    if (!LinearDraw._isLinearDraw)
                    {
                        //ラインゲージの引き直し分をゲージにプラス、初回は0
                        LineGaugeController.Instance.preslider.fillAmount = LineGaugeController.Instance.slider.fillAmount;
                        currentDis = 0;
                        LineGaugeController.AbleDraw = true;
                    }

                    _isHoldClicking = true;
                    _shotLineCamPos = lineCamera.transform.position;

                    CreateLine();

                    break;

                case TouchPhase.Moved when _isHoldClicking:
                    // 射線が固定されていたら処理しない（描きながら射撃した際にも止める）
                    if (DrawingData.IsFixed)
                    {
                        _isHoldClicking = false;
                        _currentFingerId = -1;

                        break;
                    }

                    List<Vector3> drawingFingerPos = DrawingData.FingerPositions;

                    if (Vector2.Distance(tmpFingerPos, drawingFingerPos[drawingFingerPos.Count - 1]) > lineFineness)
                    {
                        UpdateLine(tmpFingerPos);
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
        mousePos.z += lineZPos;
        Vector3 tmpMousePos = _camera.ScreenToWorldPoint(mousePos);

        if (Input.GetMouseButtonDown(0))
        {
            // UIをクリックした際は反応させない
            if (EventSystem.current.IsPointerOverGameObject()) return;

            if (LinearDraw._isLinearDraw)
            {
                //リニアドローオンの場合
                _isHoldClicking = false;

                if (!_firstLinearDraw)
                {
                    UpdateLine(tmpMousePos);
                }
            }
            else
            {
                // 画面中央からのドローのみ受け付ける
                if (Vector2.Distance(_screenCenterPos, mousePos) > drawableAreaRadius) return;

                //ラインゲージ回復不可
                LineGaugeController._isHeal = false;

                //リニアドローオフの場合
                _isHoldClicking = true;
            }

            //ラインゲージの引き直し分をゲージにプラス、初回は0
            if (!LinearDraw._isLinearDraw)
            {
                LineGaugeController.Instance.preslider.fillAmount = LineGaugeController.Instance.slider.fillAmount;
                currentDis = 0;
                LineGaugeController.AbleDraw = true;
            }

            _shotLineCamPos = lineCamera.transform.position;

            CreateLine();

        }
        else if (Input.GetMouseButton(0) && _isHoldClicking && !LinearDraw._isLinearDraw)
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
            //ラインゲージ回復可能
            LineGaugeController._isHeal = true;
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
            if (i == 0 && LinearDraw._isLinearDraw)
            {
                //0でリニアドローだったら
                DrawingData.Renderer.SetPosition(0, centerPos.transform.position);
                continue;
            }
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
    private void CreateLine()
    {

        if (LineGaugeController.AbleDraw)
        {

            SoundManager.Instance.PlaySE(SELabel.Draw);


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
                targetData = InstantiateNewLineData();
                DrawingData = targetData;
            }
            else
            {
                DrawingData = targetData;
                if (!LinearDraw._isLinearDraw)
                {
                    ShotLineUtil.ClearLineData(DrawingData);
                }
            }

            // タップ位置に起点を設定
            List<Vector3> targetDataFingerPositions = targetData.FingerPositions;
            if (!LinearDraw._isLinearDraw || _firstLinearDraw)
            {
                targetDataFingerPositions.Clear();
                targetDataFingerPositions.Add(worldTouchPos);
                targetDataFingerPositions.Add(worldTouchPos);
            }

            //リニアドローで射線をかく場合
            if (LinearDraw._isLinearDraw)
            {
                if (_firstLinearDraw)
                {
                    targetData.FingerPositions[0] = centerPos.transform.position;
                    targetDataFingerPositions[0] = centerPos.transform.position;

                    float rdis = 0;
                    float dis = Vector3.Distance(targetDataFingerPositions[0], targetDataFingerPositions[1]);
                    bool isDraw = LineGaugeController.LineGauge(dis, ref rdis);
                    currentDis += dis / LineGaugeController.Instance.MaxLinePower;
                    LineGaugeController.holdAmount = LineGaugeController.Instance.preslider.fillAmount;

                    if (!isDraw)
                    {
                        targetDataFingerPositions[1] = Vector3.Lerp(targetDataFingerPositions[0], targetDataFingerPositions[1], rdis / dis);
                    }

                    targetData.Renderer.SetPosition(0, targetDataFingerPositions[0]);
                    targetData.Renderer.SetPosition(1, targetDataFingerPositions[1]);
                    targetData.Renderer.enabled = true;
                    targetData.IsFixed = false;
                    _firstLinearDraw = false;
                }
            }
            else
            {
                targetData.Renderer.SetPosition(0, targetDataFingerPositions[0]);
                targetData.Renderer.SetPosition(1, targetDataFingerPositions[1]);
                targetData.Renderer.enabled = true;
                targetData.IsFixed = false;
            }

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
            if (DrawingData == null && !LinearDraw._isLinearDraw)
            {
                Debug.LogError("ドロー中の射線データがありません");

                return;
            }

            float dis = Vector3.Distance(DrawingData.FingerPositions[DrawingData.FingerPositions.Count - 1], newFingerPos);
            bool isDraw = LineGaugeController.LineGauge(dis, ref rdis);
            currentDis += dis / LineGaugeController.Instance.MaxLinePower;
            LineGaugeController.holdAmount = LineGaugeController.Instance.preslider.fillAmount;

            if (!isDraw)
            {
                newFingerPos = Vector3.Lerp(DrawingData.FingerPositions[DrawingData.FingerPositions.Count - 1], newFingerPos, rdis / dis);
            }

            if(!(DrawingData.FingerPositions[DrawingData.FingerPositions.Count - 1] == newFingerPos))
            {
                DrawingData.FingerPositions.Add(newFingerPos);
                DrawingData.Renderer.positionCount++;
                DrawingData.Renderer.SetPosition(DrawingData.Renderer.positionCount - 1, newFingerPos);
            }
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
