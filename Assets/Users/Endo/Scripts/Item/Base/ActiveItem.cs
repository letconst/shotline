using UnityEngine;

public abstract class ActiveItem : ItemBase
{
    protected override void Start()
    {
        base.Start();

        ItemManager.Instance.ItemBtn.onClick.AddListener(() => OnClickButton());
    }
    
    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (other.CompareTag("Player"))
        {
            Init();
        }
    }

    /// <summary>
    /// アイテムボタンをタップした際に動作させる処理
    /// </summary>
    protected abstract void OnClickButton();
}
