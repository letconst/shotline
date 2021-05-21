using UnityEngine;

public abstract class ItemBase : MonoBehaviour
{
    [SerializeField]
    private ItemData data;

    public ItemData Data => data;

    private bool _isEnabled;
    private bool _isInitialized;

    private void Update()
    {
        if (!_isEnabled) return;

        if (!_isInitialized)
        {
            Init();
            _isInitialized = true;
        }

        UpdateFunction();
    }

    /// <summary>
    /// アイテムを使用開始した際の動作
    /// </summary>
    public virtual void Init()
    {
        _isEnabled = true;
    }

    /// <summary>
    /// 毎フレームのアイテムの動作
    /// </summary>
    protected abstract void UpdateFunction();

    /// <summary>
    /// アイテムの使用が終了した際の動作
    /// </summary>
    protected virtual void Terminate()
    {
        _isEnabled     = false;
        _isInitialized = false;
    }
}
