public class ItemInitRequest : RequestBase
{
    public byte  MaxItemGenerateCount;
    public float ItemGenerateInterval;

    public ItemInitRequest()
    {
        SetType(EventType.ItemInit);
        MaxItemGenerateCount = ItemManager.MaxGenerateCount;
        ItemGenerateInterval = ItemManager.GenerateInterval;
    }
}
