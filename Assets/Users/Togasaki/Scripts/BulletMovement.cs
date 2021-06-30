using UniRx;
using UnityEngine;

public class BulletMovement : MonoBehaviour
{
    public bool BBOn = false;

    private int _selfInstanceId;

    private void Awake()
    {
        _selfInstanceId = GetInstanceID();

        // 生成されたことを相手に通知
        SendData data = MakeSendData();
        data.Self.bullet.isGenerated = true;

        NetworkManager.Emit(data);

        transform.ObserveEveryValueChanged(x => x.position)
                 .Subscribe(OnPositionChanged)
                 .AddTo(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("Shield"))
        {
            BBOn = true;
        }
    }

    private void OnDestroy()
    {
        // 破棄されたことを相手に通知
        SendData data = MakeSendData();
        data.Self.bullet.isDestroyed = true;

        NetworkManager.Emit(data);
    }

    /// <summary>
    /// 弾の座標が移動したことを相手に通知する
    /// </summary>
    /// <param name="_"></param>
    private void OnPositionChanged(Vector3 _)
    {
        SendData data = MakeSendData();

        NetworkManager.Emit(data);
    }

    private SendData MakeSendData()
    {
        return new SendData(EventType.BulletMove)
        {
            Self = new PlayerData
            {
                bullet = new BulletData
                {
                    instanceId = _selfInstanceId,
                    position   = transform.position,
                    scale      = transform.localScale
                }
            }
        };
    }
}
