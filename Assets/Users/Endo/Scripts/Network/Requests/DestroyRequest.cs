public class DestroyRequest : RequestBase
{
    public string ObjectGuid;

    public DestroyRequest(string objectGuid)
    {
        SetType(EventType.Destroy);
        ObjectGuid = objectGuid;
    }
}
