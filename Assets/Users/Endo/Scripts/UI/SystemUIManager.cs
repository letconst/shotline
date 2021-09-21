using System;
using UnityEngine;
using UnityEngine.UI;

public enum StatusText
{
    ConnectionFailed,
    TapToTitle,
    NowJoining,
    NowLoading,
    NowMatching,
    NowWaiting,
    NowWaitingOther,
    RivalDisconnected
}

public static class SystemUIManager
{
    #region Window

    private static UIWindow _openedWindow;

    /// <summary>
    /// アラートウィンドウを表示する
    /// </summary>
    /// <param name="title">タイトルテキスト</param>
    /// <param name="body">内容</param>
    /// <param name="onClose">閉じた際の処理</param>
    public static void OpenAlertWindow(string title, string body, Action onClose = null)
    {
        if (_openedWindow != null) return;

        _openedWindow = new UIWindow();
        _openedWindow.Init(onClose);
        _openedWindow.Setup(WindowMode.Alert, title, body, null);
        _openedWindow.Open();
        SetInputBlockerVisibility(true);
    }

    /// <summary>
    /// 確認（選択肢）ウィンドウを表示する
    /// </summary>
    /// <param name="title">タイトルテキスト</param>
    /// <param name="body">内容</param>
    /// <param name="onConfirm">いずれかのボタン押下時の処理。引数にどちらが押されたかの判定が渡される。true: OK, false: Cancel</param>
    /// <param name="onClose">閉じた際の処理</param>
    public static void OpenConfirmWindow(string title, string body, Action<bool> onConfirm, Action onClose = null)
    {
        if (_openedWindow != null) return;

        _openedWindow = new UIWindow();
        _openedWindow.Init(onClose);
        _openedWindow.Setup(WindowMode.Confirm, title, body, onConfirm);
        _openedWindow.Open();
        SetInputBlockerVisibility(true);
    }

    /// <summary>
    /// 開いているウィンドウ情報を破棄する
    /// </summary>
    public static void ClearWindow()
    {
        _openedWindow = null;
    }

    #endregion

    #region StatusText

    private static readonly string[] StatusTexts =
    {
        "サーバーに接続できませんでした", "タップでタイトルに戻る", "参加中", "ロード中", "マッチング中", "待機中", "他のプレイヤーを待っています", "対戦相手が切断しました"
    };

    /// <summary>
    /// 画面右下にステータステキストを表示する
    /// </summary>
    /// <param name="text">表示するテキスト</param>
    /// <param name="isReaderAnimate">テキストの直後に三点リーダーを付けるか</param>
    public static void ShowStatusText(string text, bool isReaderAnimate = true)
    {
        SystemProperty.StatusText.text = text;
        SystemProperty.StatusTextReader.SetActive(isReaderAnimate);
    }

    /// <summary>
    /// 画面右下にステータステキストを表示する
    /// </summary>
    /// <param name="status">表示するテキストの種類</param>
    /// <param name="isReaderAnimate">テキストの直後に三点リーダーを付けるか</param>
    public static void ShowStatusText(StatusText status, bool isReaderAnimate = true)
    {
        ShowStatusText(StatusTexts[(int) status], isReaderAnimate);
    }

    /// <summary>
    /// 画面右下のステータステキストを非表示にする
    /// </summary>
    public static void HideStatusText()
    {
        SystemProperty.StatusText.text = "";
        SystemProperty.StatusTextReader.SetActive(false);
    }

    #endregion

    /// <summary>
    /// 入力阻止オブジェクトの有効・無効を設定する
    /// </summary>
    /// <param name="isVisible">有効か</param>
    public static async void SetInputBlockerVisibility(bool isVisible)
    {
        Image inputBlockerImage = SystemProperty.InputBlockerImage;

        if (isVisible)
        {
            inputBlockerImage.gameObject.SetActive(true);
            await FadeTransition.FadeOut(inputBlockerImage, .2f, inputBlockerImage.color.a, .5f);
        }
        else
        {
            await FadeTransition.FadeIn(inputBlockerImage, .2f, inputBlockerImage.color.a);
            inputBlockerImage.gameObject.SetActive(false);
        }
    }
}
