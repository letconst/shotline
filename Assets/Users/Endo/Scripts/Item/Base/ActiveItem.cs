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

        ItemManager.ItemBtn.onClick.RemoveAllListeners();
        ItemManager.ItemBtn.onClick.AddListener(() => OnClickButton());
        NumQuantity.FA = 0;
        NumQuantity.Instance.im.fillAmount = NumQuantity.FA;
        LinearDraw._isLinearDraw = false;
    }

    /// <summary>
    /// アイテムボタンをタップした際に動作させる処理
    /// </summary>
    protected virtual void OnClickButton()
    {
        SoundManager.Instance.PlaySE(SELabel.Use);
    }
}
