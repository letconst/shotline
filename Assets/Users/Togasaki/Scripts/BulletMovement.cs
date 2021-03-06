using UniRx;
using UnityEngine;

public class BulletMovement : MonoBehaviour
{
    public bool BBOn = false;

    private int  _selfInstanceId;
    private bool _destroyParticleEnabled;

    private void Awake()
    {
        if (!NetworkManager.IsConnected) return;

        _selfInstanceId = GetInstanceID();

        // 生成されたことを相手に通知
        BulletMoveRequest req = MakeSendData();
        req.IsGenerated = true;

        NetworkManager.Emit(req);

        transform.ObserveEveryValueChanged(x => x.position)
                 .Subscribe(OnPositionChanged)
                 .AddTo(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("Shield"))
        {
            // エフェクトおよびSE再生
            SoundManager.Instance.PlaySE(SELabel.electric_chain, .5f);
            Instantiate(MainGameController.bulletCollideParticle, transform.position, Quaternion.identity);

            BBOn                    = true;
            _destroyParticleEnabled = true;
        }
    }

    private void OnDestroy()
    {
        if (!NetworkManager.IsConnected) return;

        // 破棄されたことを相手に通知
        BulletMoveRequest req = MakeSendData();
        req.IsDestroyed            = true;
        req.DestroyParticleEnabled = _destroyParticleEnabled;

        NetworkManager.Emit(req);
    }

    /// <summary>
    /// 弾の座標が移動したことを相手に通知する
    /// </summary>
    /// <param name="_"></param>
    private void OnPositionChanged(Vector3 _)
    {
        BulletMoveRequest data = MakeSendData();

        NetworkManager.Emit(data);
    }

    private BulletMoveRequest MakeSendData()
    {
        return new BulletMoveRequest(_selfInstanceId, transform.position, transform.rotation, transform.localScale);
    }
}
