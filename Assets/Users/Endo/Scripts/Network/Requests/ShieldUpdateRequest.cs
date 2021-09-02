public class ShieldUpdateRequest : InRoomRequestBase
{
    public string ObjectGuid;

    public ShieldUpdateRequest(string objectGuid) : base(EventType.ShieldUpdate)
    {
        ObjectGuid = objectGuid;
    }
}
