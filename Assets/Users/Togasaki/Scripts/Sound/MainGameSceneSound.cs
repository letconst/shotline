using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameSceneSound : BasicSoundManager
{
    //•ÏX‚Ì—]’n‚ ‚è
    private void Start()
    {
        PlayBGM(BGMLabel.MainGame);
    }
}
