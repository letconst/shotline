public class MatchRequest : RequestBase
{
    public string Uuid;
    public bool   IsOwner;

    public MatchRequest()
    {
        SetType(EventType.Match);
        Uuid = SelfPlayerData.Uuid;
    }
}
