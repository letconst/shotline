using System;
using System.Collections.Generic;
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
        _rivalBulletPrefab = MainGameController.rivalBulletPrefab;
    }

    private void OnReceived(object res)
    {
        var @base = (RequestBase) res;
        var type  = (EventType) Enum.Parse(typeof(EventType), @base.Type);

        if (type != EventType.BulletMove) return;

        var innerRes = (BulletMoveRequest) res;

        // 生成された弾なら新規生成
        // TODO: プーリング
        if (innerRes.IsGenerated)
        {
            GameObject newBullet = Instantiate(_rivalBulletPrefab, innerRes.Position, innerRes.Rotation);
            newBullet.transform.localScale = innerRes.Scale;
            newBullet.tag                  = "RivalBullet";

            _rivalBulletObjects.Add(innerRes.InstanceId, newBullet);

            return;
        }

        // 破棄された弾なら破棄
        if (innerRes.IsDestroyed)
        {
            int targetId = 0;

            // 破棄対象を検索
            foreach (KeyValuePair<int, GameObject> rivalBullet in _rivalBulletObjects)
            {
                if (rivalBullet.Key != innerRes.InstanceId) continue;

                Destroy(rivalBullet.Value);
                targetId = rivalBullet.Key;

                break;
            }

            _rivalBulletObjects.Remove(targetId);

            return;
        }

        // 通常移動した弾なら対象の座標を更新
        foreach (KeyValuePair<int, GameObject> rivalBullet in _rivalBulletObjects)
        {
            if (rivalBullet.Key != innerRes.InstanceId) continue;

            Transform bulletTrf = rivalBullet.Value.transform;
            bulletTrf.position   = innerRes.Position;
            bulletTrf.rotation   = innerRes.Rotation;
            bulletTrf.localScale = innerRes.Scale;

            break;
        }
    }
}
