﻿using UnityEngine;

public class TitleScript : MonoBehaviour
{
    private bool _isNowLoading;

    private void Awake()
    {
        Time.timeScale = 1;
    }

    private void Start()
    {
        SoundManager.Instance.PlayBGM(BGMLabel.Title);
    }

    private async void Update()
    {
        if (SystemSceneManager.IsLoading) return;

        bool isTouched = false;

        // タッチされたかの判定を取る (Mobile / Editor)
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                isTouched = true;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            isTouched = true;
        }

        // タッチされたらルーム選択へ遷移
        if (!_isNowLoading && isTouched && !SystemLoader.IsFirstFading)
        {
            _isNowLoading = true;

            SoundManager.Instance.PlaySE(SELabel.Start,0.5f);
            await SystemSceneManager.LoadNextScene("RoomSelection", SceneTransition.Fade);
        }
    }
}
