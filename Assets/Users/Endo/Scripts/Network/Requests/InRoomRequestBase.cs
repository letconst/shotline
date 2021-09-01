public class InRoomRequestBase : RequestBase
{
    public string RoomUuid;

    public InRoomRequestBase(EventType? type = null) : base(type)
    {
    }
}
