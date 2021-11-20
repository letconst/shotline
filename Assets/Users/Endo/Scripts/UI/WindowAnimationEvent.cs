using System;
using UnityEngine;

public class WindowAnimationEvent : MonoBehaviour
{
    private Action _onClose;

    /// <summary>
    /// 閉じるアニメーション終了時に行う処理を設定する
    /// </summary>
    /// <param name="onClose">行う処理</param>
    public void SetWindowCloseAction(Action onClose)
    {
        _onClose = onClose;
    }

    /// <summary>
    /// アニメーションタイムラインで設定するイベント
    /// </summary>
    public void OnAnimationEnd()
    {
        _onClose?.Invoke();
    }
}
