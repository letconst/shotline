public class DestroyRequest : InRoomRequestBase
{
    public string ObjectGuid;

    public DestroyRequest(string objectGuid) : base(EventType.Destroy)
    {
        ObjectGuid = objectGuid;
    }
}
