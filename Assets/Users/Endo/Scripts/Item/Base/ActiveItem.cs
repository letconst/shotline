public abstract class ActiveItem : ItemBase
{
    protected override void Start()
    {
        base.Start();

        ItemManager.Instance.ItemBtn.onClick.AddListener(OnClickButton);
    }

    /// <summary>
    /// アイテムボタンをタップした際に動作させる処理
    /// </summary>
    protected virtual void OnClickButton()
    {
        Init();
    }
}
