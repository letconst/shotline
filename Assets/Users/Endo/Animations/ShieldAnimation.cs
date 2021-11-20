using Cysharp.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ShieldAnimation : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    private static readonly int Break = Animator.StringToHash("Break");

    /// <summary>
    /// 破壊アニメーションを再生する
    /// </summary>
    public async UniTask PlayBreakAnimation()
    {
        animator.SetTrigger(Break);

        await UniTask.WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"));
        await UniTask.WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Take 001"));
    }
}
