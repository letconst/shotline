using UnityEngine;

/// <summary>
/// 特定の射線に対して色々操作するためのクラス
/// </summary>
public static class ShotLineUtil
{
    /// <summary>
    /// 射線データを作成する
    /// </summary>
    /// <param name="prefab">射線プレハブ</param>
    /// <returns>作成された射線データ</returns>
    public static LineData InstantiateLine(GameObject prefab)
    {
        var newData = new LineData(prefab);

        return newData;
    }

    /// <summary>
    /// 指定したデータの射線を固定する
    /// </summary>
    /// <param name="data">射線データ</param>
    public static void FixLine(LineData data)
    {
        data.IsFixed = true;
    }

    /// <summary>
    /// 指定した射線データの描画をクリアする
    /// </summary>
    /// <param name="data">射線データ</param>
    public static void ClearLineData(LineData data)
    {
        if (data == null) return;
        data.Renderer.positionCount = 2;
        data.Renderer.enabled       = false;
        data.IsFixed                = false;
    }

    /// <summary>
    /// 指定した射線データの使用を開放し、すべての値を初期値に戻す
    /// </summary>
    /// <param name="data">射線データ</param>
    public static void FreeLineData(LineData data)
    {
        if (data == null) return;

        ClearLineData(data);
        data.FingerPositions.Clear();
    }

    /// <summary>
    /// 指定した射線データの、射線の軌道座標を取得する
    /// </summary>
    /// <param name="data">対象の射線データ</param>
    /// <returns>射線の軌道座標</returns>
    public static Vector3[] GetFingerPositions(LineData data)
    {
        Vector3[] fingerPositions = new Vector3[data.Renderer.positionCount];
        data.Renderer.GetPositions(fingerPositions);

        return fingerPositions;
    }
}
