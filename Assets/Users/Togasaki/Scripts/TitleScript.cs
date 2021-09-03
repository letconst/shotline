using System;
using UnityEngine;
using UnityEngine.UI;

public class TitleScript : MonoBehaviour
{
    private bool _isNowLoading;

    private void Awake()
    {
        Time.timeScale = 1;
    }

    private async void Update()
    {
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

            SoundManager.Instance.PlaySE(SELabel.Start);
            await SystemSceneManager.LoadNextScene("RoomSelection", SceneTransition.Fade);
        }
    }
}
