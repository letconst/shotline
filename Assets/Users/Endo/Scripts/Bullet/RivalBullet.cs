using UnityEngine;

public class RivalBullet
{
    public int        InstanceId;
    public GameObject BulletObject;
    public Vector3    PrevFramePos;
    public int        StuckFrames;
    public bool       DestroyParticleEnabled;

    public RivalBullet(int id, GameObject obj)
    {
        InstanceId   = id;
        BulletObject = obj;
    }
}
