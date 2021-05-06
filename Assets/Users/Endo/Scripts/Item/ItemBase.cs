using UnityEngine;

public abstract class ItemBase : MonoBehaviour
{
    // TODO: Scriptableでデータベース管理するかも

    [SerializeField]
    private string itemName;

    public string ItemName { get => itemName; private set => itemName = value; }

    /// <summary>
    /// アイテムを取得・使用した際の動作
    /// </summary>
    protected abstract void Function();
}
