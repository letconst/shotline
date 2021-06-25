using UnityEngine;
using UnityEngine.UI;

public class ItemManager : SingletonMonoBehaviour<ItemManager>
{
    [SerializeField]
    private Button itemBtn;

    [SerializeField]
    private Button shotBtn;

    [SerializeField, Header("アイテム取得後、画面外への退避先の座標")]
    private Transform holdPos;

    [Header("浮遊アニメーションが1往復する秒数")]

    [SerializeField, Header("ドロップアイテムのアニメーション関連")]
    private float itemFloatingAnimDuration;

    [SerializeField, Header("浮遊アニメーションの移動量")]
    private float itemFloatingAnimScale;

    [SerializeField, Header("回転アニメーション時間 (角度/秒)")]
    private float itemRotationAnimDuration = 100;

    [SerializeField, Header("ビックバレットの最大量")]
    private float maxNumBigBullet = 3;

    [SerializeField, Header("シールドの最大量")]
    private float maxNumShield = 1;

    [SerializeField, Header("スラスターの最大量")]
    private float maxNumSluster = 1;

    public static float currentNum = 0;

    public static Image   ItemIcon { get; private set; }
    public static Button  ItemBtn  => Instance.itemBtn;
    public static Button  ShotBtn  => Instance.shotBtn;
    public static Vector3 HoldPos  => Instance.holdPos.position;

    public static float ItemFloatingAnimDuration => Instance.itemFloatingAnimDuration;
    public static float ItemFloatingAnimScale    => Instance.itemFloatingAnimScale;
    public static float ItemRotationAnimDuration => Instance.itemRotationAnimDuration;

    public static float MaxNumBigBullet => Instance.maxNumBigBullet;
    public static float MaxNumShield => Instance.maxNumShield;
    public static float MaxNumSluster => Instance.maxNumSluster;


    private static ItemBase _holdItem;


    protected override void Awake()
    {
        base.Awake();

        ItemIcon = ItemBtn.GetComponentsInChildren<Image>()[1];

        ItemIcon.sprite = null;
        ItemIcon.color  = Color.clear;

        currentNum = 0;

    }

    /// <summary>
    /// プレイヤーの所持アイテムを設定する。
    /// すでに持っているアイテムがある場合は、そちらのTerminate()を呼んでから上書き。
    /// </summary>
    /// <param name="item">持たせるアイテム</param>
    public static void SetHoldItem(ItemBase item)
    {
        currentNum = 0;

        if (_holdItem != null) _holdItem.Terminate();

        _holdItem = item;

        if (item.name == "BigBullet")
        {
            NumQuantity.maxNum = MaxNumBigBullet;
        }
        if (item.name == "ShieldItem")
        {
            NumQuantity.maxNum = MaxNumShield;
        }

    }

}
