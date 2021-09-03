public class ItemInitRequest : InRoomRequestBase
{
    public byte  MaxItemGenerateCount;
    public float ItemGenerateInterval;

    public ItemInitRequest() : base(EventType.ItemInit)
    {
        MaxItemGenerateCount = ItemManager.MaxGenerateCount;
        ItemGenerateInterval = ItemManager.GenerateInterval;
    }
}
