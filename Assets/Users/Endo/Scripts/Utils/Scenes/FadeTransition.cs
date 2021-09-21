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
    /// <param name="startAlpha">開始するアルファ値</param>
    /// <param name="endAlpha">終了するアルファ値</param>
    public static async UniTask FadeIn(CanvasGroup canvasGroup,    float fadeSpeed = DefaultFadeSpeed,
                                       float       startAlpha = 1, float endAlpha  = 0)
    {
        canvasGroup.alpha = startAlpha;

        while (canvasGroup.alpha > endAlpha)
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
    /// <param name="startAlpha">開始するアルファ値</param>
    /// <param name="endAlpha">終了するアルファ値</param>
    public static async UniTask FadeOut(CanvasGroup canvasGroup,    float fadeSpeed = DefaultFadeSpeed,
                                        float       startAlpha = 0, float endAlpha  = 1)
    {
        canvasGroup.alpha                      = startAlpha;
        SystemProperty.FadeImage.raycastTarget = true;

        while (canvasGroup.alpha < endAlpha)
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
    /// <param name="startAlpha">開始するアルファ値</param>
    /// <param name="endAlpha">終了するアルファ値</param>
    public static async UniTask FadeOut(Text  text, float fadeSpeed = DefaultFadeSpeed, float startAlpha = 1,
                                        float endAlpha = 0)
    {
        await FadeIn(text, fadeSpeed, startAlpha, endAlpha);
    }

    /// <summary>
    /// テキスト用フェードアウト処理
    /// </summary>
    /// <param name="text">対象のText</param>
    /// <param name="fadeSpeed">フェードが終了するまでの時間（秒）</param>
    /// <param name="startAlpha">開始するアルファ値</param>
    /// <param name="endAlpha">終了するアルファ値</param>
    public static async UniTask FadeIn(Text  text, float fadeSpeed = DefaultFadeSpeed, float startAlpha = 0,
                                       float endAlpha = 1)
    {
        await FadeOut(text, fadeSpeed, startAlpha, endAlpha);
    }

    /// <summary>
    /// Graphic用フェードイン処理
    /// </summary>
    /// <param name="target">Graphicを継承する対象</param>
    /// <param name="fadeSpeed">フェードが終了するまでの時間（秒）</param>
    /// <param name="startAlpha">開始するアルファ値</param>
    /// <param name="endAlpha">終了するアルファ値</param>
    public static async UniTask FadeIn(Graphic target, float fadeSpeed = DefaultFadeSpeed, float startAlpha = 1,
                                       float   endAlpha = 0)
    {
        var   color = new Color(target.color.r, target.color.g, target.color.b, startAlpha);
        float speed = 1 / fadeSpeed * (startAlpha - endAlpha);

        while (color.a > endAlpha)
        {
            color.a      -= Time.unscaledDeltaTime * speed;
            color.a      =  Mathf.Clamp01(color.a);
            target.color =  color;

            await UniTask.Yield(PlayerLoopTiming.Update);
        }

        SystemProperty.FadeImage.raycastTarget = false;
    }

    /// <summary>
    /// Graphic用フェードアウト処理
    /// </summary>
    /// <param name="target">Graphicを継承する対象</param>
    /// <param name="fadeSpeed">フェードが終了するまでの時間（秒）</param>
    /// <param name="startAlpha">開始するアルファ値</param>
    /// <param name="endAlpha">終了するアルファ値</param>
    public static async UniTask FadeOut(Graphic target, float fadeSpeed = DefaultFadeSpeed, float startAlpha = 0,
                                        float   endAlpha = 1)
    {
        var   color = new Color(target.color.r, target.color.g, target.color.b, startAlpha);
        float speed = 1 / fadeSpeed * (endAlpha - startAlpha);
        SystemProperty.FadeImage.raycastTarget = true;

        while (color.a < endAlpha)
        {
            color.a      += Time.unscaledDeltaTime * speed;
            color.a      =  Mathf.Clamp01(color.a);
            target.color =  color;

            await UniTask.Yield(PlayerLoopTiming.Update);
        }
    }
}
