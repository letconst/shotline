using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : SingletonMonoBehaviour<ItemManager>, IManagedMethod
{
    [SerializeField]
    private Button itemBtn;

    [SerializeField]
    private Button shotBtn;

    [SerializeField, Header("最大生成個数"), Range(0, 255)]
    private byte maxGenerateCount;

    [SerializeField, Header("生成される時間間隔 (秒)"), Range(0, 255)]
    private float generateInterval;

    [SerializeField, Header("アイテム取得後、画面外への退避先の座標")]
    private Transform holdPos;

    [Header("浮遊アニメーションが1往復する秒数")]
    [SerializeField, Header("ドロップアイテムのアニメーション関連")]
    private float itemFloatingAnimDuration;

    [SerializeField, Header("浮遊アニメーションの移動量")]
    private float itemFloatingAnimScale;

    [SerializeField, Header("回転アニメーション時間 (角度/秒)")]
    private float itemRotationAnimDuration = 100;

    public static float currentNum = 0;

    public static Image   ItemIcon { get; private set; }
    public static Button  ItemBtn  => Instance.itemBtn;
    public static Button  ShotBtn  => Instance.shotBtn;
    public static Vector3 HoldPos  => Instance.holdPos.position;

    public static byte  MaxGenerateCount => Instance.maxGenerateCount;
    public static float GenerateInterval => Instance.generateInterval;

    public static float ItemFloatingAnimDuration => Instance.itemFloatingAnimDuration;
    public static float ItemFloatingAnimScale    => Instance.itemFloatingAnimScale;
    public static float ItemRotationAnimDuration => Instance.itemRotationAnimDuration;
    public static sbyte GeneratedPointIndex      { get; private set; }

    public static List<GeneratedItem> GeneratedItems { get; private set; }

    private static ItemBase   _holdItem;
    private static GameObject _holdItemObj;

    public void ManagedStart()
    {
        GeneratedItems = new List<GeneratedItem>();
        ItemIcon       = ItemBtn.GetComponentsInChildren<Image>()[1];

        ItemIcon.sprite = null;
        ItemIcon.color  = Color.clear;

        currentNum   = 0;
        _holdItem    = null;
        _holdItemObj = null;

        // 初期配置されたアイテムがあればStartを呼ぶ
        GameObject[] defaultSpawnItems = GameObject.FindGameObjectsWithTag("Item");

        foreach (GameObject itemObj in defaultSpawnItems)
        {
            var item = itemObj.GetComponent<IManagedMethod>();

            item.ManagedStart();
            GeneratedItems.Add(new GeneratedItem
            {
                itemObject    = itemObj,
                managedMethod = item,
                index         = -1
            });
        }
    }

    public void ManagedUpdate()
    {
        foreach (GeneratedItem item in GeneratedItems)
        {
            item.managedMethod.ManagedUpdate();
        }
    }

    /// <summary>
    /// プレイヤーの所持アイテムを設定する。
    /// すでに持っているアイテムがある場合は、そちらのTerminate()を呼んでから上書き。
    /// </summary>
    /// <param name="item">持たせるアイテム</param>
    /// <param name="obj">アイテムオブジェクト</param>
    public static void SetHoldItem(ItemBase item, GameObject obj)
    {
        currentNum = 0;

        if (_holdItem != null) _holdItem.Terminate();

        _holdItem    = item;
        _holdItemObj = obj;
    }

    /// <summary>
    /// ランダムな位置にランダムなアイテムを生成する
    /// </summary>
    public static void GenerateRandomItem()
    {
        List<ItemPositionData> spawnPoints = MainGameProperty.ItemSpawnPoints;
        ItemPositionData       spawnPoint  = null;

        // 未生成位置が出るまで選出
        while (spawnPoint == null || spawnPoint.isSpawned)
        {
            GeneratedPointIndex = (sbyte) Random.Range(0, spawnPoints.Count);
            spawnPoint          = spawnPoints[GeneratedPointIndex];
        }

        ItemData item = ItemDatabase.GetRandomItem();

        // アイテムを生成し、初期化実行
        GameObject itemObject        = Instantiate(item.ItemObject, spawnPoint.Position, Quaternion.identity);
        var        itemManagedMethod = itemObject.GetComponent<IManagedMethod>();

        itemManagedMethod.ManagedStart();

        GeneratedItems.Add(new GeneratedItem
        {
            itemObject    = itemObject,
            managedMethod = itemManagedMethod,
            index         = GeneratedPointIndex
        });

        // 生成位置にアイテム情報を設定
        spawnPoint.itemObject = itemObject;
        spawnPoint.isSpawned  = true;
    }

    /// <summary>
    /// 指定したアイテムオブジェクトを管轄下から削除し、破棄する
    /// </summary>
    /// <param name="itemObj"></param>
    public static void DestroyItem(GameObject itemObj)
    {
        GeneratedItem removeTarget = null;

        foreach (GeneratedItem item in GeneratedItems)
        {
            if (item.itemObject != itemObj) continue;

            removeTarget = item;

            break;
        }

        if (removeTarget == null)
        {
            Debug.LogWarning("削除対象がリストにありません");

            return;
        }

        DestroyItem(removeTarget);
    }

    /// <summary>
    /// 生成情報からアイテムを破棄する
    /// </summary>
    /// <param name="item">対象アイテムの生成情報</param>
    /// <param name="isRemoveFromList">管轄リストから削除するか</param>
    private static void DestroyItem(GeneratedItem item, bool isRemoveFromList = true)
    {
        if (isRemoveFromList)
        {
            GeneratedItems.Remove(item);
        }

        Destroy(item.itemObject);
        item.managedMethod = null;
        item.itemObject    = null;
    }

    /// <summary>
    /// 所持アイテムオブジェクトを管轄下から削除し、破棄する
    /// </summary>
    public static void DestroyHoldItem()
    {
        DestroyItem(_holdItemObj);

        _holdItem    = null;
        _holdItemObj = null;
    }

    /// <summary>
    /// 生成されているアイテムをすべて破棄する
    /// </summary>
    public static void ClearGeneratedItem()
    {
        // 所持アイテムがある場合は破棄
        if (_holdItem != null) _holdItem.Terminate();

        _holdItem    = null;
        _holdItemObj = null;

        foreach (GeneratedItem item in GeneratedItems)
        {
            DestroyItem(item, false);
        }

        GeneratedItems.Clear();

        // シールドが出現している場合はすべて破棄
        GameObject[] shields = GameObject.FindGameObjectsWithTag("Shield");

        foreach (GameObject shield in shields)
        {
            Destroy(shield);
        }
    }

    /// <summary>
    /// 指定したアイテムオブジェクトの管轄下でのインデックスを取得する
    /// </summary>
    /// <param name="itemObj">アイテムオブジェクト</param>
    /// <returns>インデックス</returns>
    public static sbyte GetItemIndex(GameObject itemObj)
    {
        sbyte result = -1;

        foreach (GeneratedItem item in GeneratedItems)
        {
            if (item.itemObject != itemObj) continue;

            result = item.index;
        }

        return result;
    }
}

public class GeneratedItem
{
    public GameObject     itemObject;
    public IManagedMethod managedMethod;
    public sbyte          index;
}
