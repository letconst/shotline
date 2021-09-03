public class RoundUpdateRequest : InRoomRequestBase
{
    public string RivalUuid;
    public bool   IsLoseRival;
    public bool   IsReadyAttackedRival;

    public RoundUpdateRequest(bool isReadyAttackedRival = false) : base(EventType.RoundUpdate)
    {
        RivalUuid            = SelfPlayerData.PlayerUuid;
        IsReadyAttackedRival = isReadyAttackedRival;
    }
}
