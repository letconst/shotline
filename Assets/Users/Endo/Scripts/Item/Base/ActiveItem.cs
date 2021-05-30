using UnityEngine;

public abstract class ActiveItem : ItemBase
{
    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (other.CompareTag("Player"))
        {
            Init();
        }
    }

    protected override void Init()
    {
        base.Init();

        ItemManager.Instance.ItemBtn.onClick.RemoveAllListeners();
        ItemManager.Instance.ItemBtn.onClick.AddListener(() => OnClickButton());
    }

    /// <summary>
    /// アイテムボタンをタップした際に動作させる処理
    /// </summary>
    protected abstract void OnClickButton();
}
