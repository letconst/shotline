using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static List<ItemData> ItemList { get; private set; }

    private void Awake()
    {
        // アイテムデータを一斉ロード
        ItemData[] dataFiles = Resources.LoadAll<ItemData>("ItemData");

        ItemList = new List<ItemData>(dataFiles);
    }

    /// <summary>
    /// ランダムにアイテムデータを取得する
    /// </summary>
    /// <returns>アイテムデータ</returns>
    public static ItemData GetRandomItem()
    {
        return ItemList[Random.Range(0, ItemList.Count)];
    }
}
