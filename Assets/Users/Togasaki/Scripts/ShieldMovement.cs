using System;
using UniRx;
using UnityEngine;

public class ShieldMovement : MonoBehaviour
{
    [SerializeField]
    private int ShieldLimit;

    private string _guid;

    [SerializeField]
    private ShieldAnimation shieldAnim;

    private void Start()
    {
        ItemManager.currentShieldCount++;

        if (NetworkManager.IsConnected)
        {
            _guid = NetworkManager.GetGuid(gameObject);

            NetworkManager.OnReceived
                          ?.ObserveOnMainThread()
                          .Subscribe(OnReceived)
                          .AddTo(this);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
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

    private async void DecreaseLimit()
    {
        ShieldLimit--;

        if (ShieldLimit <= 0)
        {
            await shieldAnim.PlayBreakAnimation();

            Destroy(gameObject);
        }
    }
}
