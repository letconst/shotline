public class ItemGetRequest : InRoomRequestBase
{
    public sbyte GeneratedPointIndex;

    public ItemGetRequest(sbyte generatedPointIndex) : base(EventType.ItemGet)
    {
        GeneratedPointIndex = generatedPointIndex;
    }
}
