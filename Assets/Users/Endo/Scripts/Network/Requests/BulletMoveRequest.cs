using UnityEngine;

public class BulletMoveRequest : RequestBase
{
    public int  InstanceId;
    public bool IsGenerated;
    public bool IsDestroyed;

    public Vector3    Position;
    public Quaternion Rotation;
    public Vector3    Scale;

    public BulletMoveRequest(int  instanceId,          Vector3 position, Quaternion rotation, Vector3 scale,
                             bool isGenerated = false, bool    isDestroyed = false)
    {
        SetType(EventType.BulletMove);
        InstanceId  = instanceId;
        IsGenerated = isGenerated;
        IsDestroyed = isDestroyed;
        Position    = position;
        Rotation    = rotation;
        Scale       = scale;
    }
}
