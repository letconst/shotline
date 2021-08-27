using UnityEngine;

public class InstantiateRequest : RequestBase
{
    public string     PrefabName;
    public string     ObjectGuid;
    public Vector3    Position;
    public Quaternion Rotation;

    public InstantiateRequest(string prefabName, string objectGuid, Vector3 position, Quaternion rotation)
    {
        SetType(EventType.Instantiate);
        PrefabName = prefabName;
        ObjectGuid = objectGuid;
        Position   = position;
        Rotation   = rotation;
    }
}
