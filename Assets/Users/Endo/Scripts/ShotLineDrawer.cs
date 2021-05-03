using System.Collections.Generic;
using UnityEngine;

public class ShotLineDrawer : SingletonMonoBehaviour<ShotLineDrawer>
{
    [SerializeField, Header("射線オブジェクト")]
    private GameObject linePrefab;

    [SerializeField, Header("射線のZ位置")]
    private float lineZPos = 9.8f;

    [SerializeField, Header("射線の細かさ"), Range(.1f, 1)]
    private float lineFineness = .1f;

    private Camera        _camera;
    private GameObject    _shotLine;        // 射線オブジェクト
    private List<Vector3> _fingerPositions; // 描画された射線の通過位置
    private LineRenderer  _lineRenderer;

    private void Start()
    {
        _camera          = Camera.main;
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

        #region 射線描画操作

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            CreateLine();
        }
        else if (Input.GetMouseButton(0))
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
#elif UNITY_IOS
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);

            switch (t.phase)
            {
                case TouchPhase.Began:
                    CreateLine();

                    break;

                case TouchPhase.Moved:
                    Vector3 touchPos = t.position;
                    touchPos.z += lineZPos;
                    Vector3 tmpFingerPos = _camera.ScreenToWorldPoint(touchPos);

                    if (Vector2.Distance(tmpFingerPos, _fingerPositions[_fingerPositions.Count - 1]) > lineFineness)
                    {
                        UpdateLine(tmpFingerPos);
                    }

                    break;
            }
        }
#endif

        #endregion
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
