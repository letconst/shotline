using System;
using UnityEngine;

// この武器に決定するかの確認ボタン
public class WeaponConfirmDialog : MonoBehaviour
{
    ScrollSnapSelector snapSelector;

    public enum ConfirmResult
    {
        OK,
        Cancel,
    }

    private void Start()
    {
        snapSelector = GameObject.FindWithTag("ScrollView").GetComponent<ScrollSnapSelector>();
    }

    // ダイアログが操作されたときに発生するイベント
    public Action<ConfirmResult> FixDialog { get; set; }

    // OKボタンが押されたとき
    public void OnOk()
    {
        this.FixDialog?.Invoke(ConfirmResult.OK);
        WeaponManager.SelectWeapon = WeaponManager.weaponDatas[snapSelector.hIndex -1];
        Destroy(this.gameObject);
    }

    // Cancelボタンが押されたとき
    public void OnCancel()
    {
        // イベント通知先があれば通知してダイアログを破棄してしまう
        this.FixDialog?.Invoke(ConfirmResult.Cancel);
        Destroy(this.gameObject);
    }
}