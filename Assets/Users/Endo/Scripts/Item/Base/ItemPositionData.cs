using UnityEngine;

/// <summary>
/// アイテムの生成位置情報（Bolt用）
/// </summary>
public class ItemPositionData : MonoBehaviour
{
    /// <summary>
    /// 生成位置。
    /// </summary>
    public Vector3 Position => transform.position;

    /// <summary>
    /// この場所にアイテムが生成されているか。
    /// </summary>
    public bool isSpawned;

    private GameObject SpawnedItem;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            isSpawned = true;
            SpawnedItem = other.gameObject;
        }
    }


}
