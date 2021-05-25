using UnityEngine;

public abstract class PassiveItem : ItemBase
{
    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (other.CompareTag("Player"))
        {
            Init();
        }
    }
}
