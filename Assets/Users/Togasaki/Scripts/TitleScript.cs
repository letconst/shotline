using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScript : MonoBehaviour
{
    //マッチ用bool
    private bool MatchSuccess = true;

    private bool _isNowLoading;

    private void Update()
    {
        bool isTouched = false;

        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                isTouched = true;
            }
        }

        // タッチ入力でメインシーンへ遷移
        if (!_isNowLoading && (Input.GetMouseButtonDown(0) || isTouched) && !SystemLoader.IsFirstFading)
        {
            _isNowLoading = true;
            SystemSceneManager.LoadNextScene("MainGameScene", SceneTransition.Fade);
        }
    }

    //タップしたときの処理
    public void ChangeSceneToGame()
    {
        //マッチングの処理を下に

        //マッチングできたら"MainGameScene"を開く
        if (MatchSuccess)
        {
            SceneManager.LoadScene("MainGameScene");
        }
    }
}
