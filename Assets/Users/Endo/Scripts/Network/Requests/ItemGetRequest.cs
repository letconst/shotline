public class ItemGetRequest : RequestBase
{
    public sbyte GeneratedPointIndex;

    public ItemGetRequest(sbyte generatedPointIndex)
    {
        SetType(EventType.ItemGet);
        GeneratedPointIndex = generatedPointIndex;
    }
}
