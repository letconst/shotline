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
    public static async UniTask LoadNextScene(string sceneName, SceneTransition type,
                                              float  fadeSpeed = FadeTransition.DefaultFadeSpeed)
    {
        // 別のシーンをロード中は読み込まない
        if (IsLoading) return;

        IsLoading = true;

        switch (type)
        {
            case SceneTransition.Fade:
            {
                await FadeTransition.FadeOut(SystemProvider.FadeCanvasGroup, fadeSpeed);
                await UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
                await FadeTransition.FadeIn(SystemProvider.FadeCanvasGroup, fadeSpeed);

                break;
            }

            default:
                throw new System.ArgumentOutOfRangeException(nameof(type), type, null);
        }

        IsLoading = false;
    }
}
