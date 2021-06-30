using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameSceneSound : SoundManager
{
    private void Start()
    {
        PlayBGM(BGMLabel.MainGame);
    }
}
