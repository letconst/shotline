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

    /// <summary>
    /// アイテムが生成されてからの経過時間。
    /// この変数に秒数を代入すると、自動的に時間が減少していき、ゼロ以下になるとゼロに固定されます。
    /// </summary>
    public float elapsedTimeFromSpawned;

    private GameObject SpawnedItem;

    private void Update()
    {
        if (elapsedTimeFromSpawned > 0)
        {
            elapsedTimeFromSpawned -= Time.deltaTime;
        }
        else
        {
            elapsedTimeFromSpawned = 0;
        }
        if (SpawnedItem == null)
        {
            isSpawned = false;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            isSpawned = true;
            SpawnedItem = other.gameObject;
        }
    }

   
}
