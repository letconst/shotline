using UnityEngine;
using UnityEngine.UI;

public abstract class ItemBase : MonoBehaviour
{
    [SerializeField]
    private ItemData data;

    public ItemData Data => data;

    private bool  _isEnabled;
    private bool  _isInitialized;
    private Image _itemIcon;

    protected virtual void Start()
    {
        _itemIcon = ItemManager.Instance.ItemIcon;
    }

    private void Update()
    {
        if (!_isEnabled) return;

        if (!_isInitialized)
        {
            Init();
        }

        UpdateFunction();
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // UIへの反映
            _itemIcon.sprite = Data.ItemSprite;
            _itemIcon.color  = Color.white;

            // 画面外への移動
            transform.position = ItemManager.Instance.HoldPos;
        }
    }

    /// <summary>
    /// アイテムを取得した際の動作
    /// </summary>
    protected virtual void Init()
    {
        if (_isInitialized) return;

        _isEnabled     = true;
        _isInitialized = true;
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
        _isEnabled = false;
        ClearItemIcon();
        Destroy(this);
    }

    /// <summary>
    /// UIに描画されたアイテムアイコンをクリアする
    /// </summary>
    protected void ClearItemIcon()
    {
        _itemIcon.sprite = null;
        _itemIcon.color  = Color.clear;
    }
}
