using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameSceneSound : BasicSoundManager
{
    private void Start()
    {
        PlayBGM(BGMLabel.MainGame);
    }
}
