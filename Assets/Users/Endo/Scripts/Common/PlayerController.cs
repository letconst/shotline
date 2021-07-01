using UniRx;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private void Awake()
    {
        // 1P設定
        if (NetworkManager.IsOwner)
        {
            GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/Bullet_PL1");
        }
        // 2P設定
        else
        {
            GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/Bullet_PL2");
        }

        if (NetworkManager.IsConnected)
        {
            // 座標が更新されたらサーバーに座標更新通信
            transform.ObserveEveryValueChanged(x => x.position)
                     .Subscribe(OnPositionChanged)
                     .AddTo(this);
        }
    }

    private void OnPositionChanged(Vector3 pos)
    {
        var data = new SendData(EventType.PlayerMove)
        {
            Self = new PlayerData
            {
                Position = pos,
                Rotation = transform.rotation
            }
        };

        NetworkManager.Emit(data);
    }
}
