public class ItemInitRequest : RequestBase
{
    public byte  MaxItemGenerateCount;
    public float ItemGenerateInterval;

    public ItemInitRequest() : base(EventType.ItemInit)
    {
        MaxItemGenerateCount = ItemManager.MaxGenerateCount;
        ItemGenerateInterval = ItemManager.GenerateInterval;
    }
}
