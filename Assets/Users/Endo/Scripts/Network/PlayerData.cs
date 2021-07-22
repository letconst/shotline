using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string Name;
    public string Uuid;
    public string Address;
    public ushort Port;

    public Vector3    Position;
    public Quaternion Rotation;
    public Vector3    Scale;

    public BulletData bullet;

    // TODO: 相打ちになった際の判定のために、UNIX時間等も乗せて鯖で判定してあげる
    public bool isLose;

    public PlayerData()
    {
        Uuid    = SelfPlayerData.Uuid;
        Address = SelfPlayerData.Address;
        Port    = SelfPlayerData.Port;
    }
}

public static class SelfPlayerData
{
    public static string Name;
    public static string Uuid;
    public static string Address;
    public static ushort Port;
}
