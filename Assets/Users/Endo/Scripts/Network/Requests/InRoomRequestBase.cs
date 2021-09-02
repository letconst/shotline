public abstract class InRoomRequestBase : RequestBase
{
    public string RoomUuid;
    public string ClientUuid;

    public InRoomRequestBase(EventType? type = null) : base(type)
    {
        RoomUuid   = SelfPlayerData.RoomUuid;
        ClientUuid = SelfPlayerData.PlayerUuid;
    }
}
