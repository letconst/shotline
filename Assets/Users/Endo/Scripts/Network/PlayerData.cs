﻿using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string Name;
    public string Uuid;
    public string Address;
    public int    Port;

    public Vector3    Position;
    public Quaternion Rotation;
    public Vector3    Scale;

    public BulletData bullet;

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
    public static int    Port;
}
