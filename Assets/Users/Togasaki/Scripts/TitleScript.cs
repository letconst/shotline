﻿using System;
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
    private const string LoadingText          = "ロード中…";

    private void Awake()
    {
        Time.timeScale = 1;
        _statusText    = SystemProperty.StatusText;
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
                SoundManager.Instance.PlaySE(SELabel.Start);

                // TODO: 通信中UI表示
                TitleProperty.StatusBgImage.SetActive(true);
                _statusText.enabled = true;
                _statusText.text    = ConnectingText;

                // サーバーへの接続待機
                await NetworkManager.Connect();

                // 初期接続
                var req = new InitRequest();
                NetworkManager.Emit(req);

                _isInitConnected = true;
            }
        }
        else
        {
            if (!_isNowLoading && (Input.GetMouseButtonDown(0) || isTouched) && !SystemLoader.IsFirstFading)
            {
                SoundManager.Instance.PlaySE(SELabel.Start);
                _isNowLoading = true;
                await SystemSceneManager.LoadNextScene("MainGameScene", SceneTransition.Fade);
            }
        }
    }

    private async void OnReceived(object res)
    {
        var @base = (RequestBase) res;
        var type  = (EventType) Enum.Parse(typeof(EventType), @base.Type);

        switch (type)
        {
            // 初期接続完了時
            case EventType.Init:
            {
                var req = new MatchRequest();

                NetworkManager.Emit(req);
                _statusText.text = MatchingText;

                break;
            }

            case EventType.Match:
            {
                _receiver.Dispose();

                // TODO: 部屋への参加完了UI表示
                _statusText.text = MatchingCompleteText;

                await UniTask.Delay(TimeSpan.FromSeconds(1.5f));
                await UniTask.SwitchToMainThread();

                _statusText.text = LoadingText;

                await SystemSceneManager.LoadNextScene("MainGameScene", SceneTransition.Fade);

                break;
            }

            case EventType.Error:
            {
                var innerRes = (ErrorRequest) res;

                // TODO: UIでエラー表示
                _statusText.text = innerRes.Message;

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
