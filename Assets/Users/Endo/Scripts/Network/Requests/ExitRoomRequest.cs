public class ExitRoomRequest : InRoomRequestBase
{
    public bool   IsExitable;
    public string Message;

    public ExitRoomRequest() : base(EventType.ExitRoom)
    {
    }
}
