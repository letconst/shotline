using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameSceneSound : BasicSoundManager
{
    //変更の余地あり
    private void Start()
    {
        PlayBGM(BGMLabel.MainGame);
    }
}
