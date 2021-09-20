using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class RivalBulletManager : MonoBehaviour
{
    private GameObject _rivalBulletPrefab;

    private List<RivalBullet>        _rivalBullets;
    private Queue<RivalBullet>       _bulletInstantiateQueue;
    private Queue<int>               _bulletDestroyQueue;
    private Queue<BulletMoveRequest> _bulletMoveQueue;

    private void Awake()
    {
        _rivalBullets           = new List<RivalBullet>();
        _bulletInstantiateQueue = new Queue<RivalBullet>();
        _bulletDestroyQueue     = new Queue<int>();
        _bulletMoveQueue        = new Queue<BulletMoveRequest>();

        if (!NetworkManager.IsConnected) return;

        NetworkManager.OnReceived
                      ?.ObserveOnMainThread()
                      .Subscribe(OnReceived)
                      .AddTo(this);
    }

    private void Start()
    {
        _rivalBulletPrefab = MainGameController.rivalBulletPrefab;
    }

    private void Update()
    {
        // 生成されたものがあれば管理下に追加
        _rivalBullets.AddRange(_bulletInstantiateQueue);
        _bulletInstantiateQueue.Clear();

        // 破棄されたものがあれば管理化から削除
        foreach (int id in _bulletDestroyQueue)
        {
            RivalBullet targetBullet = null;

            foreach (RivalBullet bullet in _rivalBullets)
            {
                if (bullet.InstanceId != id) continue;

                targetBullet = bullet;
            }

            if (targetBullet == null)
            {
                Debug.LogWarning($"{nameof(RivalBulletManager)}: 破棄対象の弾が見つかりません");

                continue;
            }

            // 弾オブジェクトがあれば破棄
            if (targetBullet.BulletObject != null)
            {
                Destroy(targetBullet.BulletObject);
            }

            _rivalBullets.Remove(targetBullet);
        }

        _bulletDestroyQueue.Clear();

        // 弾のスタッキングを確認
        foreach (RivalBullet bullet in _rivalBullets)
        {
            // 弾オブジェクトがなければ破棄対象に
            if (bullet.BulletObject == null)
            {
                _bulletDestroyQueue.Enqueue(bullet.InstanceId);

                continue;
            }

            Vector3 bulletPos = bullet.BulletObject.transform.position;

            // 1フレーム前と同じ位置ならカウント++
            if (bulletPos == bullet.PrevFramePos)
            {
                bullet.StuckFrames++;
            }
            // 更新されていればカウントリセット
            else
            {
                bullet.StuckFrames  = 0;
                bullet.PrevFramePos = bulletPos;

                continue;
            }

            // 0.5秒移動していなかったら自動的に破棄
            if (bullet.StuckFrames == Application.targetFrameRate / 2)
            {
                _bulletDestroyQueue.Enqueue(bullet.InstanceId);
            }
        }

        // 各弾の座標更新
        while (_bulletMoveQueue.Count > 0)
        {
            BulletMoveRequest res = _bulletMoveQueue.Dequeue();

            foreach (RivalBullet bullet in _rivalBullets)
            {
                // 同じIDの弾のみ処理
                if (bullet.InstanceId != res.InstanceId) continue;

                // 破棄されてたら処理しない
                if (bullet.BulletObject == null) continue;

                Transform bulletTrf = bullet.BulletObject.transform;
                bulletTrf.SetPositionAndRotation(res.Position, res.Rotation);
                bulletTrf.localScale = res.Scale;

                break;
            }
        }
    }

    private void OnReceived(object res)
    {
        var @base = (RequestBase) res;
        var type  = (EventType) System.Enum.Parse(typeof(EventType), @base.Type);

        if (type != EventType.BulletMove) return;

        var innerRes = (BulletMoveRequest) res;

        // 生成された弾なら新規生成
        // TODO: プーリング
        if (innerRes.IsGenerated)
        {
            GameObject newBulletObj = Instantiate(_rivalBulletPrefab, innerRes.Position, innerRes.Rotation);
            newBulletObj.transform.localScale = innerRes.Scale;
            newBulletObj.tag                  = "RivalBullet";

            var newBullet = new RivalBullet(innerRes.InstanceId, newBulletObj);

            _bulletInstantiateQueue.Enqueue(newBullet);

            return;
        }

        // 破棄された弾なら破棄
        if (innerRes.IsDestroyed)
        {
            _bulletDestroyQueue.Enqueue(innerRes.InstanceId);

            return;
        }

        // 通常移動した弾なら対象の座標を更新
        _bulletMoveQueue.Enqueue(innerRes);
    }
}
