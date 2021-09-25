using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShotLineDrawer : SingletonMonoBehaviour<ShotLineDrawer>, IManagedMethod
{
    [SerializeField, Header("射線のZ位置")]
    private float lineZPos = 9.8f;

    [SerializeField, Header("射線の太さ"), Range(.1f, 1)]
    private float lineFineness = .1f;

    [SerializeField, Header("射線を引ける領域の半径")]
    private float drawableAreaRadius;

    [SerializeField, Header("ゲーム開始時、事前に生成しておく射線オブジェクトの数")]
    private int initPoolingCount = 5;

    private       GameObject _linePrefab;
    private       bool       _isHoldClicking; // 射線を描いている最中か
    private       Camera     _camera;
    private       Vector3    _touchedPlayerPos; // タップ開始時のプレイヤーの位置
    private       Vector3    _prevPlayerPos;    // 1フレーム前のプレイヤーの位置
    private       Vector2    _screenCenterPos;  // 画面の中心位置
    private       int        _currentFingerId;  // 現在射線を描いている指ID
    public static float      currentDis;        //最新のゲージ消費量
    public static bool       _firstLinearDraw;  //二回目以降のリニアドローか

    private int _drawLineLayerMask;
    private int _lineGaugeLayerMask;

    private Transform _playerTrf;

    private List<LineData> _lineDataList;

    /// <summary>
    /// ドロー中の射線データ。ドロー開始時から射撃が終了するまで保持される。
    /// </summary>
    public static LineData DrawingData { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        _camera           = Camera.main;
        _touchedPlayerPos = Vector3.zero;
        _prevPlayerPos    = Vector3.zero;
        _screenCenterPos  = new Vector2(Screen.width / 2f, Screen.height / 2f);
        _currentFingerId  = -1;
        _lineDataList     = new List<LineData>();
        DrawingData       = null;
        currentDis        = 0;
        _firstLinearDraw  = true;

        _drawLineLayerMask  = 1 << 8;
        _lineGaugeLayerMask = 1 << 10;
    }

    public void ManagedStart()
    {
        _linePrefab = MainGameController.linePrefab;
        _playerTrf  = GameObject.FindGameObjectWithTag("Player").transform;

        // 射線データ生成
        for (int i = 0; i < initPoolingCount; i++)
        {
            _lineDataList.Add(new LineData(_linePrefab));
        }
    }

    public void ManagedUpdate()
    {
        if (Input.touchCount > 0)
        {
            MobileInputHandler();
        }
        else
        {
            PCInputHandler();
        }

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
                // タッチ開始時の処理
                // タッチ判定および射線描画を開始させ、判定中のfingerIdを保持する
                case TouchPhase.Began:
                {
                    Ray ray = _camera.ScreenPointToRay(touchPos);

                    // 描画開始領域のクリックのみ反応させる。リニアガン時は無視
                    if (!Physics.Raycast(ray, Mathf.Infinity, _lineGaugeLayerMask) && !LinearDraw._isLinearDraw) return;

                    // UIをクリックした際は反応させない
                    if (EventSystem.current.IsPointerOverGameObject(touch.fingerId)) return;

                    // 射線の描画領域をタップしたときのみ処理
                    if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _drawLineLayerMask)) return;

                    if (LinearDraw._isLinearDraw)
                    {
                        //リニアドローオンの場合
                        _isHoldClicking = false;

                        if (!_firstLinearDraw)
                        {
                            UpdateLine(hit.point);
                        }
                    }
                    else
                    {
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
                        LineGaugeController.Instance.preslider.fillAmount =
                            LineGaugeController.Instance.slider.fillAmount;

                        currentDis                   = 0;
                        LineGaugeController.AbleDraw = true;
                    }

                    _touchedPlayerPos = _playerTrf.position;

                    CreateLine(hit.point);

                    break;
                }

                // タッチ最中の処理
                // 射線描画の更新を行う
                case TouchPhase.Moved when _isHoldClicking:
                {
                    // 射線が固定されていたら処理しない（描きながら射撃した際にも止める）
                    if (DrawingData.IsFixed)
                    {
                        _isHoldClicking  = false;
                        _currentFingerId = -1;

                        break;
                    }

                    //ラインゲージ回復不可
                    LineGaugeController._isHeal = false;

                    List<Vector3> drawingFingerPos = DrawingData.FingerPositions;
                    Vector3       tmpTouchPos      = touchPos;
                    tmpTouchPos.z += 1;

                    Vector3 worldFingerPos = _camera.ScreenToWorldPoint(tmpTouchPos);

                    if (Vector2.Distance(worldFingerPos, drawingFingerPos[drawingFingerPos.Count - 1]) > lineFineness)
                    {
                        Ray ray = _camera.ScreenPointToRay(touchPos);

                        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _drawLineLayerMask))
                        {
                            // 移動中は、移動開始時のプレイヤー位置との差分を引いた位置に描画する
                            Vector3 hitPos         = hit.point;
                            Vector3 deltaPlayerPos = _touchedPlayerPos - _playerTrf.position;
                            hitPos += deltaPlayerPos;

                            UpdateLine(hitPos);
                        }
                    }

                    break;
                }

                // タッチ終了時の処理
                // タッチ判定を破棄する
                case TouchPhase.Ended:
                {
                    _isHoldClicking  = false;
                    _currentFingerId = -1;
                    //ラインゲージ回復可能
                    LineGaugeController._isHeal = true;

                    break;
                }
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
            Ray ray = _camera.ScreenPointToRay(mousePos);

            // 描画開始領域のクリックのみ反応させる。リニアガン時は無視
            if (!Physics.Raycast(ray, Mathf.Infinity, _lineGaugeLayerMask) && !LinearDraw._isLinearDraw) return;

            // UIをクリックした際は反応させない
            if (EventSystem.current.IsPointerOverGameObject()) return;

            // 射線の描画領域をクリックしたときのみ処理
            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _drawLineLayerMask)) return;

            if (LinearDraw._isLinearDraw)
            {
                //リニアドローオンの場合
                _isHoldClicking = false;

                if (!_firstLinearDraw)
                {
                    UpdateLine(hit.point);
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
                currentDis                                        = 0;
                LineGaugeController.AbleDraw                      = true;
            }

            _isHoldClicking   = true;
            _touchedPlayerPos = _playerTrf.position;

            CreateLine(hit.point);
        }
        else if (Input.GetMouseButton(0) && _isHoldClicking && !LinearDraw._isLinearDraw)
        {
            // 1つ前の射線位置から指定距離離れていたら伸ばす
            List<Vector3> drawingFingerPos = DrawingData.FingerPositions;
            Vector3       tmpMousePos      = mousePos;
            tmpMousePos.z += lineZPos;
            Vector3 worldMousePos = _camera.ScreenToWorldPoint(tmpMousePos);

            if (Vector2.Distance(worldMousePos, drawingFingerPos[drawingFingerPos.Count - 1]) > lineFineness)
            {
                Ray ray = _camera.ScreenPointToRay(mousePos);

                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _drawLineLayerMask))
                {
                    UpdateLine(hit.point);
                }
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

        Vector3 curPlayerPos = _playerTrf.position;

        // プレイヤーの位置が1フレーム前と同じなら処理しない
        if (curPlayerPos == _prevPlayerPos) return;

        // 射線のドロー開始時と現在のプレイヤーの差分座標
        Vector3 deltaPosToPlayer = curPlayerPos - _touchedPlayerPos;

        List<Vector3> drawingFingerPos     = DrawingData.FingerPositions;
        int           fingerPositionsCount = drawingFingerPos.Count;

        // 射線の全ポイントの座標を更新
        for (int i = 0; i < fingerPositionsCount; i++)
        {
            if (i == 0 && LinearDraw._isLinearDraw)
            {
                //0でリニアドローだったら
                DrawingData.Renderer.SetPosition(0, _playerTrf.position);

                continue;
            }

            Vector3 newPos = drawingFingerPos[i] + deltaPosToPlayer;
            DrawingData.Renderer.SetPosition(i, newPos);
        }

        _prevPlayerPos = curPlayerPos;
    }

    /// <summary>
    /// 射線を作成する
    /// </summary>
    private void CreateLine(Vector3 newFingerPosition)
    {
        // 射線ゲージが空のときは処理しない
        if (!LineGaugeController.AbleDraw) return;

        SoundManager.Instance.PlaySE(SELabel.Draw);

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
            targetData  = InstantiateNewLineData();
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
        List<Vector3> targetDataFingerPositions = DrawingData.FingerPositions;

        if (!LinearDraw._isLinearDraw || _firstLinearDraw)
        {
            targetDataFingerPositions.Clear();
            targetDataFingerPositions.Add(newFingerPosition);
            targetDataFingerPositions.Add(newFingerPosition);
        }

        // リニアドローの場合
        if (LinearDraw._isLinearDraw)
        {
            if (_firstLinearDraw)
            {
                DrawingData.FingerPositions[0] = _playerTrf.position;
                targetDataFingerPositions[0]   = _playerTrf.position;

                float rdis   = 0;
                float dis    = Vector3.Distance(targetDataFingerPositions[0], targetDataFingerPositions[1]);
                bool  isDraw = LineGaugeController.LineGauge(dis, ref rdis);
                currentDis                     += dis / LineGaugeController.Instance.MaxLinePower;
                LineGaugeController.holdAmount =  LineGaugeController.Instance.preslider.fillAmount;

                if (!isDraw)
                {
                    targetDataFingerPositions[1] = Vector3.Lerp(targetDataFingerPositions[0],
                                                                targetDataFingerPositions[1], rdis / dis);
                }
                else
                {
                    targetDataFingerPositions[1] = newFingerPosition;
                }

                DrawingData.Renderer.SetPosition(0, targetDataFingerPositions[0]);
                DrawingData.Renderer.SetPosition(1, targetDataFingerPositions[1]);
                DrawingData.Renderer.enabled = true;
                DrawingData.IsFixed          = false;
                _firstLinearDraw             = false;
            }
        }
        // 通常描画
        else
        {
            DrawingData.Renderer.SetPosition(0, newFingerPosition);
            DrawingData.Renderer.SetPosition(1, newFingerPosition);
            DrawingData.Renderer.enabled = true;
            DrawingData.IsFixed          = false;
        }
    }

    /// <summary>
    /// ドロー中の射線を伸ばす
    /// </summary>
    /// <param name="newFingerPos">追加する位置</param>
    private void UpdateLine(Vector3 newFingerPos)
    {
        // 射線ゲージが空のときは処理しない
        if (!LineGaugeController.AbleDraw) return;

        if (DrawingData == null && !LinearDraw._isLinearDraw)
        {
            Debug.LogError("ドロー中の射線データがありません");

            return;
        }

        float rdis = 0;
        float dis = Vector3.Distance(DrawingData.FingerPositions[DrawingData.FingerPositions.Count - 1],
                                     newFingerPos);

        // 延長予定地に描くためのゲージ残量が足りるか判定
        bool isDraw = LineGaugeController.LineGauge(dis, ref rdis);
        currentDis                     += dis / LineGaugeController.Instance.MaxLinePower;
        LineGaugeController.holdAmount =  LineGaugeController.Instance.preslider.fillAmount;

        // ゲージ残量がなければ、残量分の長さとする
        if (!isDraw)
        {
            newFingerPos = Vector3.Lerp(DrawingData.FingerPositions[DrawingData.FingerPositions.Count - 1],
                                        newFingerPos, rdis / dis);
        }

        // 重複する位置でなければ描画延長
        if (DrawingData.FingerPositions[DrawingData.FingerPositions.Count - 1] != newFingerPos)
        {
            DrawingData.FingerPositions.Add(newFingerPos);
            DrawingData.Renderer.positionCount++;
            DrawingData.Renderer.SetPosition(DrawingData.Renderer.positionCount - 1, newFingerPos);
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
