public class JoinedRequest : RequestBase
{
    public string Uuid;

    public JoinedRequest()
    {
        SetType(EventType.Joined);
        Uuid = SelfPlayerData.Uuid;
    }
}
