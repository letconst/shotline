public class DisconnectRequest : RequestBase
{
    public string Uuid;

    public DisconnectRequest()
    {
        SetType(EventType.Disconnect);
        Uuid = SelfPlayerData.Uuid;
    }
}
