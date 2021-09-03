using UnityEngine;

public class InstantiateRequest : InRoomRequestBase
{
    public string     PrefabName;
    public string     ObjectGuid;
    public Vector3    Position;
    public Quaternion Rotation;

    public InstantiateRequest(string prefabName, string objectGuid, Vector3 position, Quaternion rotation)
        : base(EventType.Instantiate)
    {
        PrefabName = prefabName;
        ObjectGuid = objectGuid;
        Position   = position;
        Rotation   = rotation;
    }
}
