using UniRx;
using UnityEngine;

public class RivalAnimationController : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    private Vector3 _prevPlayerPos;

    private static readonly int Walk = Animator.StringToHash("Walk");
    private static readonly int Shot = Animator.StringToHash("Shot");

    private void Start()
    {
        NetworkManager.OnReceived
                      ?.ObserveOnMainThread()
                      .Subscribe(OnReceived)
                      .AddTo(this);
    }

    private void Update()
    {
        animator.SetBool(Walk, transform.position != _prevPlayerPos);

        _prevPlayerPos = transform.position;
    }

    private void OnReceived(object res)
    {
        var @base = (RequestBase) res;
        var type  = (EventType) System.Enum.Parse(typeof(EventType), @base.Type);

        // 相手の射撃開始を受信時にモーション再生
        if (type != EventType.BulletMove) return;

        var innerRes = (BulletMoveRequest) res;

        if (!innerRes.IsGenerated) return;

        animator.SetTrigger(Shot);
    }
}
