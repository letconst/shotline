using UnityEngine;

public class PlayerMoveRequest : RequestBase
{
    public Vector3    Position;
    public Quaternion Rotation;

    public PlayerMoveRequest(Vector3 position, Quaternion rotation)
    {
        SetType(EventType.PlayerMove);
        Position = position;
        Rotation = rotation;
    }
}
