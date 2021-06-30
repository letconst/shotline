using System;
using UnityEngine;

/// <summary>
/// サーバーへの送信データクラス
/// </summary>
[Serializable]
public class SendData
{
    public string     Type;
    public string     Message;
    public PlayerData Self;
    public PlayerData Rival;

    public SendData(EventType type)
    {
        Type = type.ToString();
    }

    /// <summary>
    /// 文字列を送信用のJson形式に変換する
    /// </summary>
    /// <param name="data">変換する文字列</param>
    /// <returns>変換されたデータ</returns>
    public static SendData MakeJsonFrom(string data)
    {
        try
        {
            return JsonUtility.FromJson<SendData>(data);
        }
        catch (Exception e)
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
    public static string ParseSendData(SendData data)
    {
        return JsonUtility.ToJson(data);
    }
}
