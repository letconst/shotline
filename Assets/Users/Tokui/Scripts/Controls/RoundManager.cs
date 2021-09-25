using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class RoundManager : SingletonMonoBehaviour<RoundManager>
{
    [SerializeField]
    private int PlayerLife = 3; //プレイヤーのライフ

    [SerializeField]
    private float CountDownReset;

    [SerializeField]
    private float WallCount;

    [SerializeField]
    private float WallSpeed;

    [SerializeField]
    private float WallSpeedReset;

    [SerializeField]
    private Text roundText;

    private float CountDown;

    public bool SuddenDeathFlag;

    public bool PlayerDeathFlag;

    // 現在ラウンドが切り替わっている最中かどうか判別
    public static bool RoundMove = false;

    private int  _rivalLife;
    private bool _isFadeInSuddenDeathImg;

    public static int  CurrentPlayerLife { get; private set; }
    public static int  CurrentRound      { get; private set; }
    public static Text RoundText         => Instance.roundText;

    protected void Start()
    {
        RoundMove         = false;
        CurrentPlayerLife = PlayerLife;
        _rivalLife        = PlayerLife;
        CurrentRound      = 1;

        // ラウンド進行を受信時にラウンド数更新
        NetworkManager.OnReceived
                      ?.ObserveOnMainThread()
                      .Subscribe(OnReceived)
                      .AddTo(this);
    }

    private void Update()
    {
        if (SuddenDeathFlag == true)
        {
            //外壁の縮小
            foreach (Transform wall in MainGameProperty.SotoWalls)
            {
                Vector3 sotoWallScale = wall.localScale;
                sotoWallScale.x -= WallSpeed * Time.deltaTime;
                sotoWallScale.z -= WallSpeed * Time.deltaTime;
                wall.localScale =  sotoWallScale;
            }

            // カウントダウン
            CountDown += Time.deltaTime;

            if (CountDown >= 3 && !_isFadeInSuddenDeathImg)
            {
                _isFadeInSuddenDeathImg = true;
                FadeTransition.FadeIn(MainGameProperty.SuddenDeathImg, .5f);
            }

            if (CountDown >= WallCount)
            {
                WallSpeed = WallSpeedReset;

                SuddenDeathFlag = false;

                CountDown = CountDownReset;
            }
        }
    }

    public static void HitVerification()
    {
        // プレイヤーのライフを1減らす
        CurrentPlayerLife--;

        RoundMove                = true;
        Instance.SuddenDeathFlag = false;

        // プレイヤーのライフが0になったらリザルトへ
        if (CurrentPlayerLife == 0)
        {
            //リザルトへ
            Debug.Log("Result");
        }
    }

    private void OnReceived(object res)
    {
        var @base = (RequestBase) res;
        var type  = (EventType) System.Enum.Parse(typeof(EventType), @base.Type);

        switch (type)
        {
            case EventType.RoundUpdate:
            {
                var innerRes = (RoundUpdateRequest) res;

                if (innerRes.IsReadyAttackedRival) break;

                CurrentRound++;

                break;
            }

            case EventType.SuddenDeathStart:
            {
                SuddenDeathFlag         = true;
                _isFadeInSuddenDeathImg = false;
                FadeTransition.FadeOut(MainGameProperty.SuddenDeathImg, .5f);

                break;
            }
        }
    }
}
