using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : SingletonMonoBehaviour<ItemManager>, IManagedMethod
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

    private Dictionary<GameObject, IManagedMethod> _generatedItems;

    public static float currentNum = 0;

    public static Image   ItemIcon { get; private set; }
    public static Button  ItemBtn  => Instance.itemBtn;
    public static Button  ShotBtn  => Instance.shotBtn;
    public static Vector3 HoldPos  => Instance.holdPos.position;

    public static float ItemFloatingAnimDuration => Instance.itemFloatingAnimDuration;
    public static float ItemFloatingAnimScale    => Instance.itemFloatingAnimScale;
    public static float ItemRotationAnimDuration => Instance.itemRotationAnimDuration;

    private static ItemBase _holdItem;

    public void ManagedStart()
    {
        _generatedItems = new Dictionary<GameObject, IManagedMethod>();
        ItemIcon        = ItemBtn.GetComponentsInChildren<Image>()[1];

        ItemIcon.sprite = null;
        ItemIcon.color  = Color.clear;

        currentNum = 0;

        // 初期配置されたアイテムがあればStartを呼ぶ
        GameObject[] defaultSpawnItems = GameObject.FindGameObjectsWithTag("Item");

        foreach (GameObject itemObj in defaultSpawnItems)
        {
            var item = itemObj.GetComponent<IManagedMethod>();

            item.ManagedStart();
            _generatedItems.Add(itemObj, item);
        }
    }

    public void ManagedUpdate()
    {
        foreach (KeyValuePair<GameObject, IManagedMethod> item in _generatedItems)
        {
            item.Value.ManagedUpdate();
        }
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
    }

    /// <summary>
    /// 指定したアイテムオブジェクトを管轄下から削除し、対象も破棄する
    /// </summary>
    /// <param name="itemObj"></param>
    public static void DestroyItem(GameObject itemObj)
    {
        Instance._generatedItems.Remove(itemObj);
        Destroy(itemObj);
    }
}
