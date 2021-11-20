using UnityEngine;

public static class Initializer
{
    [RuntimeInitializeOnLoadMethod]
    private static void Init()
    {
        Application.targetFrameRate = 60;

        // 表示中ウィンドウの情報がstaticなためクリアしておく
        SystemUIManager.ClearWindow();

        // サウンドマネージャー生成
        var        soundPrefab = Resources.Load<GameObject>("Prefabs/SoundManager");
        GameObject soundObject = Object.Instantiate(soundPrefab);
        soundObject.name = soundObject.name.Replace("(Clone)", "");
    }
}
