public class ShieldUpdateRequest : RequestBase
{
    public string ObjectGuid;

    public ShieldUpdateRequest(string objectGuid)
    {
        SetType(EventType.ShieldUpdate);
        ObjectGuid = objectGuid;
    }
}
