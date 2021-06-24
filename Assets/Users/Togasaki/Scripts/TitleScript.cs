using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;

public class TitleScript : MonoBehaviour
{
    //マッチ用bool
    private bool MatchSuccess = true;

    private bool _isInitConnected;
    private bool _isMatchingStarted;

    private IDisposable receiver;

    private void Start()
    {
        receiver = NetworkManager.OnReceived
                                 .Where(res => res.Type.Equals("Init") || res.Type.Equals("Join"))
                                 .Subscribe(res =>
                                 {
                                     EventType type = (EventType) Enum.Parse(typeof(EventType), res.Type);

                                     if (type == EventType.Init)
                                     {
                                         SelfPlayerData.Uuid = res.Self.Uuid;

                                         SendData data = new SendData(EventType.Join)
                                         {
                                             Self = new PlayerData
                                             {
                                                 Uuid = SelfPlayerData.Uuid
                                             }
                                         };

                                         NetworkManager.Emit(data);
                                     }
                                     else if (type == EventType.Join)
                                     {
                                         // TODO: 部屋への参加完了UI表示

                                         SystemSceneManager.LoadNextScene("MainGameScene", SceneTransition.Fade);
                                         receiver.Dispose();
                                     }
                                 });
    }

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

        if (Input.GetMouseButtonDown(0))
        {
            isTouched = true;
        }

        if (isTouched && !_isInitConnected)
        {
            NetworkManager.Connect();

            SendData data = new SendData(EventType.Init);
            NetworkManager.Emit(data);

            _isInitConnected = true;

            // TODO: 通信中UI表示
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
