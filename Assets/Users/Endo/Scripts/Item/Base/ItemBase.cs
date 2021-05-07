using UnityEngine;

public abstract class ItemBase : MonoBehaviour
{
    [SerializeField]
    private ItemData data;

    public ItemData Data => data;

    /// <summary>
    /// アイテムを取得・使用した際の動作
    /// </summary>
    protected abstract void Function();
}
