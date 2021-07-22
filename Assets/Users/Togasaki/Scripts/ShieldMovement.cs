using System;
using UniRx;
using UnityEngine;

public class ShieldMovement : MonoBehaviour
{
    private int ShieldLimit = 10;

    private string _guid;

    private void Start()
    {
        ShieldLimit = 10;
        ItemManager.currentShieldCount++;

        if (NetworkManager.IsConnected)
        {
            _guid = NetworkManager.GetGuid(gameObject);

            NetworkManager.OnReceived.Subscribe(OnReceived).AddTo(this);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet") || other.CompareTag("RivalBullet"))
        {
            if (NetworkManager.IsConnected)
            {
                var data = new SendData(EventType.ShieldUpdate)
                {
                    objectGuid = _guid
                };

                NetworkManager.Emit(data);
            }

            DecreaseLimit();
        }
    }

    private void OnDestroy()
    {
        ItemManager.currentShieldCount--;
    }

    private void OnReceived(SendData data)
    {
        var type = (EventType) Enum.Parse(typeof(EventType), data.Type);

        // シールド通信のみ処理
        if (type != EventType.ShieldUpdate) return;

        // 同一シールドのみ処理
        if (data.objectGuid != _guid) return;

        DecreaseLimit();
    }

    private void DecreaseLimit()
    {
        ShieldLimit--;

        if (ShieldLimit <= 0)
        {
            Destroy(gameObject);
        }
    }
}
