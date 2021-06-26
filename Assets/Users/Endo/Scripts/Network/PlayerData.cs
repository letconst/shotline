using UnityEngine;

public class PlayerData
{
    public string Name;
    public string Uuid;
    public string Address;
    public string Port;

    public Vector3    Position;
    public Quaternion Rotation;
    public Vector3    Scale;
}

public class SelfPlayerData
{
    public static string Name;
    public static string Uuid;
    public static string Address;
    public static int    Port;
}
