using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using UnityEngine.UI;

public class TitleScript : MonoBehaviour
{
    [SerializeField]
    private bool isDebug;

    //マッチ用bool
    private bool MatchSuccess = true;

    private bool _isInitConnected;
    private bool _isMatchingStarted;
    private bool _isNowLoading;
    private Text _statusText;

    private IDisposable _receiver;

    private const string ConnectingText       = "接続中…";
    private const string MatchingText         = "マッチング中…";
    private const string MatchingCompleteText = "マッチング完了！";

    private void Awake()
    {
        _statusText = TitleProperty.StatusText;
    }

    private void Start()
    {
        // サーバーからの受信データを処理する
        _receiver = NetworkManager.OnReceived
                                  .ObserveOnMainThread() // UIを編集するためメインスレッドで
                                  .Subscribe(OnReceived)
                                  .AddTo(this);
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

        if (isDebug)
        {
            if (isTouched && !_isInitConnected)
            {
                // TODO: 通信中UI表示
                TitleProperty.StatusBgImage.SetActive(true);
                _statusText.enabled = true;
                _statusText.text    = ConnectingText;

                // サーバーへの接続待機
                await NetworkManager.Connect();

                // 初期接続
                var data = new SendData(EventType.Init);
                NetworkManager.Emit(data);

                _isInitConnected = true;
            }
        }
        else
        {
            if (!_isNowLoading && (Input.GetMouseButtonDown(0) || isTouched) && !SystemLoader.IsFirstFading)
            {
                _isNowLoading = true;
                await SystemSceneManager.LoadNextScene("MainGameScene", SceneTransition.Fade);
            }
        }
    }

    private async void OnReceived(SendData res)
    {
        var type = (EventType) Enum.Parse(typeof(EventType), res.Type);

        switch (type)
        {
            // 初期接続完了時
            case EventType.Init:
            {
                var data = new SendData(EventType.Match)
                {
                    Self = new PlayerData()
                };

                NetworkManager.Emit(data);
                _statusText.text = MatchingText;

                break;
            }

            case EventType.Match:
            {
                _receiver.Dispose();

                // TODO: 部屋への参加完了UI表示
                _statusText.text = MatchingCompleteText;

                await UniTask.Delay(TimeSpan.FromSeconds(1.5f));

                await SystemSceneManager.LoadNextScene("MainGameScene", SceneTransition.Fade);

                break;
            }

            case EventType.Error:
            {
                // TODO: UIでエラー表示
                _statusText.text = res.Message;

                break;
            }
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
