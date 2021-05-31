using UnityEngine;
using UnityEngine.UI;

public abstract class ItemBase : MonoBehaviour
{
    [SerializeField]
    private ItemData data;

    public ItemData Data => data;

    private bool    _isEnabled;
    private bool    _isInitialized;
    private Image   _itemIcon;
    private Vector3 _basePos;

    public bool isAnimate; // アニメーションをするか

    protected virtual void Start()
    {
        _itemIcon = ItemManager.ItemIcon;
        _basePos  = transform.position;
        isAnimate = true;
    }

    private void Update()
    {
        if (isAnimate) IdleAnimation();

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
            // 所持アイテム設定
            ItemManager.SetHoldItem(this);

            // UIへの反映
            _itemIcon.sprite = Data.ItemSprite;
            _itemIcon.color  = Color.white;

            // 画面外への移動
            transform.position = ItemManager.HoldPos;

            // アニメーション停止
            isAnimate = false;
        }
    }

    private void IdleAnimation()
    {
        // 上下移動
        float freq      = 1 / ItemManager.ItemFloatingAnimDuration;
        float sin       = Mathf.Sin(2 * Mathf.PI * freq * Time.time);
        float deltaPosY = sin * ItemManager.ItemFloatingAnimScale;

        Vector3 curPos = transform.position;
        float   newY   = float.IsNaN(_basePos.y + deltaPosY) ? _basePos.y : _basePos.y + deltaPosY;
        transform.position = new Vector3(curPos.x, newY, curPos.z);

        // 回転
        transform.Rotate(new Vector3(0, ItemManager.ItemRotationAnimDuration * Time.deltaTime, 0));
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
    public virtual void Terminate()
    {
        _isEnabled = false;
        ClearItemIcon();
        ItemManager.ItemBtn.onClick.RemoveAllListeners();
        Destroy(gameObject);
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
