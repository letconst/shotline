using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum WindowMode
{
    /// <summary>
    /// アラート表示（OKボタンのみ）
    /// </summary>
    Alert,

    /// <summary>
    /// 確認表示（OK、キャンセルボタン）
    /// </summary>
    Confirm
}

public class UIWindow : UIBase
{
    private Action       _onClose;
    private Action<bool> _onConfirm;

    private Animation     _windowAnimation;
    private AnimationClip _openClip;
    private AnimationClip _closeClip;

    private Text   _titleText;
    private Text   _bodyText;
    private Button _okButton;
    private Button _cancelButton;

    private const string OpenClipName  = "Open";
    private const string CloseClipName = "Close";

    /// <summary>
    /// 各種初期化を行う
    /// </summary>
    /// <param name="onClose">ウィンドウを閉じた際の処理</param>
    public void Init(Action onClose)
    {
        base.Init();

        _onClose = onClose;

        GameObject windowObject = SystemProperty.WindowObject;
        var        windowProp   = windowObject.GetComponent<WindowProperty>();

        // 各種情報キャッシュ
        _okButton        = windowProp.okButton;
        _cancelButton    = windowProp.cancelButton;
        _titleText       = windowProp.titleText;
        _bodyText        = windowProp.bodyText;
        _windowAnimation = windowObject.GetComponent<Animation>();

        // OKボタンに閉じる処理を登録
        _okButton.onClick.AddListener(Close);

        // ウィンドウアニメーション読み込みおよびデータ登録
        _openClip  = Resources.Load<AnimationClip>("Animations/WindowOpen");
        _closeClip = Resources.Load<AnimationClip>("Animations/WindowClose");
        _windowAnimation.AddClip(_openClip, OpenClipName);
        _windowAnimation.AddClip(_closeClip, CloseClipName);

        // ウィンドウを閉じた際の処理を登録
        var winAnimEvent = windowObject.GetComponent<WindowAnimationEvent>();

        if (winAnimEvent)
        {
            winAnimEvent.SetWindowCloseAction(OnCloseAnimationEnd);
        }
    }

    /// <summary>
    /// ウィンドウを開く
    /// </summary>
    public void Open()
    {
        SystemProperty.WindowObject.SetActive(true);
        _windowAnimation.Play(OpenClipName);
    }

    /// <summary>
    /// ウィンドウを閉じる
    /// </summary>
    public void Close()
    {
        _windowAnimation.Play(CloseClipName);
        _onClose?.Invoke();
    }

    /// <summary>
    /// 閉じるアニメーション終了時の処理
    /// </summary>
    private static void OnCloseAnimationEnd()
    {
        SystemProperty.WindowObject.SetActive(false);
        SystemUIManager.SetInputBlockerVisibility(false);
        SystemUIManager.ClearWindow();
    }

    /// <summary>
    /// ウィンドウの内容や処理を設定する
    /// </summary>
    /// <param name="mode">表示モード</param>
    /// <param name="title">タイトルテキスト</param>
    /// <param name="body">内容</param>
    /// <param name="onConfirm">いずれかのボタン押下時の処理。引数にどちらが押されたかの判定が渡される。true: OK, false: Cancel</param>
    public void Setup(WindowMode mode, string title, string body, Action<bool> onConfirm)
    {
        _titleText.text = title;
        _bodyText.text  = body;
        _onConfirm      = onConfirm;

        switch (mode)
        {
            case WindowMode.Alert:
            {
                _cancelButton.gameObject.SetActive(false);

                break;
            }

            case WindowMode.Confirm:
            {
                // 各ボタンに処理設定
                _okButton.onClick.RemoveAllListeners();
                _okButton.onClick.AddListener(() => OnConfirm(true));

                _cancelButton.onClick.RemoveAllListeners();
                _cancelButton.onClick.AddListener(() => OnConfirm(false));

                break;
            }
        }
    }

    /// <summary>
    /// 確認ウィンドウ表示時の各ボタンに設定される処理
    /// </summary>
    /// <param name="result">どちらのボタンが押されたか</param>
    private void OnConfirm(bool result)
    {
        Close();
        _onConfirm?.Invoke(result);
    }
}
