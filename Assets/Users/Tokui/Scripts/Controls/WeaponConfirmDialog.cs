using System;
using UnityEngine;

public class WeaponConfirmDialog : MonoBehaviour
{
    ScrollSnapSelector scrollSnapSelector;

    // この武器に決定するかの確認ボタン
    public enum DialogResult
    {
        OK,
        Cancel,
    }

    // ダイアログが操作されたときに発生するイベント
    public Action<DialogResult> FixDialog { get; set; }

    // OKボタンが押されたとき
    public void OnOk()
    {
        this.FixDialog?.Invoke(DialogResult.OK);
        Destroy(this.gameObject);
    }

    // Cancelボタンが押されたとき
    public void OnCancel()
    {
        // イベント通知先があれば通知してダイアログを破棄してしまう
        this.FixDialog?.Invoke(DialogResult.Cancel);
        Destroy(this.gameObject);
    }
}