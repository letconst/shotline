using UnityEngine;

[System.Serializable]
public class RequestBase
{
    public string Type;

    public RequestBase(EventType? eventType = null)
    {
        if (eventType != null)
        {
            Type = eventType.ToString();
        }
    }

    protected void SetType(EventType eventType)
    {
        Type = eventType.ToString();
    }

    /// <summary>
    /// 文字列を送信用のJson形式に変換する
    /// </summary>
    /// <param name="data">変換する文字列</param>
    /// <returns>変換されたデータ</returns>
    public static T MakeJsonFrom<T>(string data) where T : RequestBase
    {
        try
        {
            return JsonUtility.FromJson<T>(data);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Jsonに変換できませんでした: {e.Message}");

            return null;
        }
    }

    /// <summary>
    /// 送信用データを文字列に変換する
    /// </summary>
    /// <param name="data">送信用データ</param>
    /// <returns>変換された文字列</returns>
    public static string ParseSendData(RequestBase data)
    {
        return JsonUtility.ToJson(data);
    }
}
