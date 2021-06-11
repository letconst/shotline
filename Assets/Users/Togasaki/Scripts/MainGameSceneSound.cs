using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameSceneSound : MonoBehaviour
{
    private void Start()
    {
        SoundManager.PlayBGM(BGMLabel.MainGame);
    }
}
