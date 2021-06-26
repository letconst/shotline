using Cysharp.Threading.Tasks;
using UnityEngine;

public class SystemLoader : MonoBehaviour
{
    private static bool       _isFirstScene; // ゲーム開始直後のシーンか
    private static GameObject _systemObject;

    public static bool IsFirstFading { get; private set; }

    private async void Start()
    {
        if (_isFirstScene) return;

        IsFirstFading = true;

        // ゲーム開始直後のシーンではフェードインする
        await UniTask.Yield(PlayerLoopTiming.Update);
        await FadeTransition.FadeIn(SystemProvider.FadeCanvasGroup);

        _isFirstScene = true;
        IsFirstFading = false;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Init()
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/GameSystem");
        _systemObject      = Instantiate(prefab);
        _systemObject.name = _systemObject.name.Replace("(Clone)", "");

        DontDestroyOnLoad(_systemObject);
    }

    private void OnApplicationQuit()
    {
        _isFirstScene = false;
    }
}
