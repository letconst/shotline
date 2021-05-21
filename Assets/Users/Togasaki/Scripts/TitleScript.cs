using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScript : MonoBehaviour
{
    //マッチ用bool
    bool MatchSuccess = false;
    
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
