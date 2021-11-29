public class PlayerMoveRequest : InRoomRequestBase
{
    public UnityEngine.Vector3    Position;
    public UnityEngine.Quaternion Rotation;

    public PlayerMoveRequest(UnityEngine.Vector3 position, UnityEngine.Quaternion rotation) : base(EventType.PlayerMove)
    {
        Position = position;
        Rotation = rotation;
    }
}
