using Cysharp.Threading.Tasks;
using UnityEngine;

public enum SceneTransition
{
    Fade = 0
}

public class SystemSceneManager : MonoBehaviour
{
    /// <summary>
    /// シーンを読み込んでいる最中か
    /// </summary>
    public static bool IsLoading { get; private set; }

    /// <summary>
    /// シーンを読み込む
    /// </summary>
    /// <param name="sceneName">読み込むシーン名</param>
    /// <param name="type">読み込みアニメーション</param>
    /// <param name="fadeSpeed">アニメーション時間（フェードのみ）</param>
    /// <param name="isShowStatus">ロード中のステータステキストを表示するか</param>
    public static async UniTask LoadNextScene(string sceneName, SceneTransition type,
                                              float  fadeSpeed    = FadeTransition.DefaultFadeSpeed,
                                              bool   isShowStatus = false)
    {
        // 別のシーンをロード中は読み込まない
        if (IsLoading) return;

        IsLoading = true;

        switch (type)
        {
            case SceneTransition.Fade:
            {
                await FadeTransition.FadeOut(SystemProperty.FadeCanvasGroup, fadeSpeed);

                if (isShowStatus) SystemUIManager.ShowStatusText(StatusText.NowLoading);

                await UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);

                if (isShowStatus) SystemUIManager.HideStatusText();

                await FadeTransition.FadeIn(SystemProperty.FadeCanvasGroup, fadeSpeed);

                break;
            }

            default:
                throw new System.ArgumentOutOfRangeException(nameof(type), type, null);
        }

        IsLoading = false;
    }
}
