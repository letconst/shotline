using UnityEngine;

public static class Initializer
{
    [RuntimeInitializeOnLoadMethod]
    private static void Init()
    {
        Application.targetFrameRate = 60;
    }
}
