using UnityEngine;

[CreateAssetMenu(fileName = "NewItemData", menuName = "New Item Data")]
public class ItemData : ScriptableObject
{
    [SerializeField]
    private string itemName;

    [SerializeField]
    private Sprite itemSprite;

    [SerializeField]
    private GameObject itemObject;

    public string     ItemName   { get => itemName;   private set => itemName = value; }
    public Sprite     ItemSprite { get => itemSprite; private set => itemSprite = value; }
    public GameObject ItemObject { get => itemObject; private set => itemObject = value; }
}
