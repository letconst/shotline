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
}
