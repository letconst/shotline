using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

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
            canvasGroup.alpha -= Time.unscaledDeltaTime * (1 / fadeSpeed);
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
            canvasGroup.alpha += Time.unscaledDeltaTime * (1 / fadeSpeed);
            canvasGroup.alpha =  Mathf.Clamp01(canvasGroup.alpha);

            await UniTask.Yield(PlayerLoopTiming.Update);
        }
    }

    /// <summary>
    /// テキスト用フェードイン処理
    /// </summary>
    /// <param name="text">対象のText</param>
    /// <param name="fadeSpeed">フェードが終了するまでの時間（秒）</param>
    public static async UniTask FadeOut(Text text, float fadeSpeed = DefaultFadeSpeed)
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, 1);

        Color tmpColor = text.color;

        while (text.color.a > 0)
        {
            tmpColor.a -= Time.unscaledDeltaTime * (1 / fadeSpeed);
            tmpColor.a =  Mathf.Clamp01(tmpColor.a);
            text.color =  tmpColor;

            await UniTask.Yield(PlayerLoopTiming.Update);
        }

        SystemProperty.FadeImage.raycastTarget = false;
    }

    /// <summary>
    /// テキスト用フェードアウト処理
    /// </summary>
    /// <param name="text">対象のText</param>
    /// <param name="fadeSpeed">フェードが終了するまでの時間（秒）</param>
    public static async UniTask FadeIn(Text text, float fadeSpeed = DefaultFadeSpeed)
    {
        text.color                             = new Color(text.color.r, text.color.g, text.color.b, 0);
        SystemProperty.FadeImage.raycastTarget = true;

        Color tmpColor = text.color;

        while (text.color.a < 1)
        {
            tmpColor.a += Time.unscaledDeltaTime * (1 / fadeSpeed);
            tmpColor.a =  Mathf.Clamp01(tmpColor.a);
            text.color =  tmpColor;

            await UniTask.Yield(PlayerLoopTiming.Update);
        }
    }
}
