using Cysharp.Threading.Tasks;
using UnityEngine;

public static class FadeTransition
{
    public const float DefaultFadeSpeed = 1;

    /// <summary>
    /// フェードイン処理
    /// </summary>
    /// <param name="canvasGroup">対象のCanvasGroup</param>
    /// <param name="fadeSpeed">フェードが終了するまでの時間（秒）</param>
    public static async UniTask FadeIn(CanvasGroup canvasGroup, float fadeSpeed = DefaultFadeSpeed)
    {
        canvasGroup.alpha = 1;

        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime * (1 / fadeSpeed);
            canvasGroup.alpha =  Mathf.Clamp01(canvasGroup.alpha);

            await UniTask.Yield(PlayerLoopTiming.Update);
        }

        SystemProperty.FadeImage.raycastTarget = false;
    }

    /// <summary>
    /// フェードアウト処理
    /// </summary>
    /// <param name="canvasGroup">対象のCanvasGroup</param>
    /// <param name="fadeSpeed">フェードが終了するまでの時間（秒）</param>
    public static async UniTask FadeOut(CanvasGroup canvasGroup, float fadeSpeed = DefaultFadeSpeed)
    {
        canvasGroup.alpha                      = 0;
        SystemProperty.FadeImage.raycastTarget = true;

        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime * (1 / fadeSpeed);
            canvasGroup.alpha =  Mathf.Clamp01(canvasGroup.alpha);

            await UniTask.Yield(PlayerLoopTiming.Update);
        }
    }
}
