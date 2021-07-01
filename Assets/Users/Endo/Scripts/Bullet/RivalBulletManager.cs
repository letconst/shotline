using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class RivalBulletManager : MonoBehaviour
{
    private GameObject _rivalBulletPrefab;

    private Dictionary<int, GameObject> _rivalBulletObjects;

    private void Awake()
    {
        if (!NetworkManager.IsConnected) return;

        _rivalBulletObjects = new Dictionary<int, GameObject>();

        NetworkManager.OnReceived
                      ?.ObserveOnMainThread()
                      .Subscribe(OnReceived)
                      .AddTo(this);
    }

    private void Start()
    {
        _rivalBulletPrefab = MainGameController.RivalBulletPrefab;
    }

    private void OnReceived(SendData data)
    {
        var type = (EventType) Enum.Parse(typeof(EventType), data.Type);

        if (type != EventType.BulletMove) return;

        BulletData bullet = data.Rival.bullet;

        // 生成された弾なら新規生成
        if (bullet.isGenerated)
        {
            GameObject newBullet = Instantiate(_rivalBulletPrefab, bullet.position, bullet.rotation);
            newBullet.transform.localScale = bullet.scale;

            _rivalBulletObjects.Add(bullet.instanceId, newBullet);

            return;
        }

        // 破棄された弾なら破棄
        if (bullet.isDestroyed)
        {
            KeyValuePair<int, GameObject>[] rivalBullets = _rivalBulletObjects.ToArray();

            foreach (KeyValuePair<int, GameObject> rivalBullet in rivalBullets)
            {
                if (rivalBullet.Key != bullet.instanceId) continue;

                Destroy(rivalBullet.Value);
                _rivalBulletObjects.Remove(rivalBullet.Key);

                break;
            }

            return;
        }

        // 通常移動した弾なら対象の座標を更新
        foreach (KeyValuePair<int, GameObject> rivalBullet in _rivalBulletObjects)
        {
            if (rivalBullet.Key != bullet.instanceId) continue;

            Transform bulletTrf = rivalBullet.Value.transform;
            bulletTrf.position   = bullet.position;
            bulletTrf.rotation   = bullet.rotation;
            bulletTrf.localScale = bullet.scale;

            break;
        }
    }
}
