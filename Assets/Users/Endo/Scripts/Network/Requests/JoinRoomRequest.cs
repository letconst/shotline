public class JoinRoomRequest : InRoomRequestBase
{
    public bool   IsJoinable;
    public Client Client;
    public string Message;
    public bool   IsOwner;

    public JoinRoomRequest()
    {
        SetType(EventType.JoinRoom);
    }
}
