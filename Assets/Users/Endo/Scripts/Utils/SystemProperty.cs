using TMPro;
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
    private GameObject windowObject;

    public static GameObject WindowObject => Instance.windowObject;

    [SerializeField]
    private GameObject inputBlocker;

    public static GameObject InputBlocker => Instance.inputBlocker;

    [SerializeField]
    private Image inputBlockerImage;

    public static Image InputBlockerImage => Instance.inputBlockerImage;

    [SerializeField]
    private TextMeshProUGUI statusText;

    public static TextMeshProUGUI StatusText => Instance.statusText;

    [SerializeField]
    private GameObject statusTextReader;

    public static GameObject StatusTextReader => Instance.statusTextReader;
}
