using System;
using System.Collections.Generic;
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

    public static readonly Queue<WindowEntry> WindowQueue = new Queue<WindowEntry>();

    /// <summary>
    /// アラートウィンドウを表示する
    /// </summary>
    /// <param name="title">タイトルテキスト</param>
    /// <param name="body">内容</param>
    /// <param name="onClose">閉じた際の処理</param>
    public static void OpenAlertWindow(string title, string body, Action onClose = null)
    {
        if (_openedWindow != null)
        {
            WindowQueue.Enqueue(new WindowEntry(WindowMode.Alert, title, body, null, onClose));

            return;
        }

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
        if (_openedWindow != null)
        {
            WindowQueue.Enqueue(new WindowEntry(WindowMode.Confirm, title, body, onConfirm, onClose));

            return;
        }

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

    private const string ConnectingText = "通信中";

    /// <summary>
    /// 画面右下にステータステキストを表示する
    /// </summary>
    /// <param name="text">表示するテキスト</param>
    /// <param name="isReaderAnimate">テキストの直後に三点リーダーを付けるか</param>
    /// <param name="withShadow">影を表示するか</param>
    public static void ShowStatusText(string text, bool isReaderAnimate = true, bool withShadow = false)
    {
        SystemProperty.StatusText.text = text;
        SystemProperty.StatusTextReader.SetActive(isReaderAnimate);

        if (withShadow)
        {
            SystemProperty.ConnectingShadow.enabled = true;
        }
    }

    /// <summary>
    /// 画面右下にステータステキストを表示する
    /// </summary>
    /// <param name="status">表示するテキストの種類</param>
    /// <param name="isReaderAnimate">テキストの直後に三点リーダーを付けるか</param>
    /// <param name="withShadow">影を表示するか</param>
    public static void ShowStatusText(StatusText status, bool isReaderAnimate = true, bool withShadow = false)
    {
        ShowStatusText(StatusTexts[(int) status], isReaderAnimate, withShadow);
    }

    /// <summary>
    /// 画面右下のステータステキストを非表示にする
    /// </summary>
    public static void HideStatusText()
    {
        SystemProperty.StatusText.text = "";
        SystemProperty.StatusTextReader.SetActive(false);
        SetInputBlockerVisibility(false);
        SystemProperty.ConnectingShadow.enabled = false;
    }

    /// <summary>
    /// 接続中のステータステキストを表示する
    /// </summary>
    public static void ShowConnectingStatus()
    {
        ShowStatusText(ConnectingText, withShadow: true);
        SetInputBlockerVisibility(true, 0);
    }

    #endregion

    /// <summary>
    /// 入力阻止オブジェクトの有効・無効を設定する
    /// </summary>
    /// <param name="isVisible">有効か</param>
    /// <param name="alpha">透明度</param>
    public static async void SetInputBlockerVisibility(bool isVisible, float alpha = .5f)
    {
        Image inputBlockerImage = SystemProperty.InputBlockerImage;

        if (isVisible)
        {
            inputBlockerImage.gameObject.SetActive(true);
            await FadeTransition.FadeIn(inputBlockerImage, .2f, inputBlockerImage.color.a, alpha);
        }
        else
        {
            await FadeTransition.FadeOut(inputBlockerImage, .2f, inputBlockerImage.color.a);
            inputBlockerImage.gameObject.SetActive(false);
        }
    }
}

public class WindowEntry
{
    public WindowMode   Mode;
    public string       Title;
    public string       Body;
    public Action<bool> OnConfirm;
    public Action       OnClose;

    public WindowEntry(WindowMode mode, string title, string body, Action<bool> onConfirm, Action onClose)
    {
        Mode      = mode;
        Title     = title;
        Body      = body;
        OnConfirm = onConfirm;
        OnClose   = onClose;
    }
}
