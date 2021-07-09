using UnityEngine;
using UnityEngine.UI;

public class SystemProperty : SingletonMonoBehaviour<SystemProperty>
{
    [SerializeField]
    private SystemSceneManager systemSceneManager;

    public static SystemSceneManager SystemSceneManager => Instance.systemSceneManager;

    [SerializeField]
    private CanvasGroup fadeCanvasGroup;

    public static CanvasGroup FadeCanvasGroup => Instance.fadeCanvasGroup;

    [SerializeField]
    private Image fadeImage;

    public static Image FadeImage => Instance.fadeImage;

    [SerializeField]
    private Text statusText;

    public static Text StatusText => Instance.statusText;
}
