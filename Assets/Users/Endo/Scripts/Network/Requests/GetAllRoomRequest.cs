public class GetAllRoomRequest : RequestBase
{
    public Room[] Rooms;

    public GetAllRoomRequest()
    {
        SetType(EventType.GetAllRoom);
    }
}
