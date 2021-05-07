using UnityEngine;

public abstract class ActiveItem : ItemBase
{
    /// <summary>
    /// このアイテム情報を所持アイテムUIに反映する
    /// Boltから呼び出し予定のためpublic
    /// </summary>
    public void DrawItemButton()
    {
    }
}
