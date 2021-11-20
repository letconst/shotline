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
                var shieldUpdateReq = new ShieldUpdateRequest(_guid);

                NetworkManager.Emit(shieldUpdateReq);
            }

            SoundManager.Instance.PlaySE(SELabel.varia);

            DecreaseLimit();
        }
    }

    private void OnDestroy()
    {
        ItemManager.currentShieldCount--;
    }

    private void OnReceived(object res)
    {
        var @base = (RequestBase) res;
        var type  = (EventType) Enum.Parse(typeof(EventType), @base.Type);

        // シールド通信のみ処理
        if (type != EventType.ShieldUpdate) return;

        var innerRes = (ShieldUpdateRequest) res;

        // 同一シールドのみ処理
        if (innerRes.ObjectGuid != _guid) return;

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
