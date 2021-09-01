public class RoundUpdateRequest : RequestBase
{
    public string RivalUuid;
    public bool   IsLoseRival;
    public bool   IsReadyAttackedRival;

    public RoundUpdateRequest(bool isReadyAttackedRival = false)
    {
        SetType(EventType.RoundUpdate);
        RivalUuid            = SelfPlayerData.PlayerUuid;
        IsReadyAttackedRival = isReadyAttackedRival;
    }
}
