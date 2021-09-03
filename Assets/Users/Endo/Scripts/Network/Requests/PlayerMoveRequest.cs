using UnityEngine;

public class PlayerMoveRequest : InRoomRequestBase
{
    public Vector3    Position;
    public Quaternion Rotation;

    public PlayerMoveRequest(Vector3 position, Quaternion rotation) : base(EventType.PlayerMove)
    {
        Position = position;
        Rotation = rotation;
    }
}
