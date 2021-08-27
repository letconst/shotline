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
    private GameObject sotowall;

    [SerializeField]
    private GameObject SuddenDeathStartText;

    [SerializeField]
    private Text roundText;

    private float CountDown;

    public static bool SuddenDeathFlag;

    public bool PlayerDeathFlag;




    // 現在ラウンドが切り替わっている最中かどうか判別
    public static bool RoundMove = false;

    public static int  CurrentPlayerLife { get; private set; }
    public static int  CurrentRound      { get; private set; }
    public static Text RoundText         => Instance.roundText;

    protected override void Awake()
    {
        base.Awake();

        RoundMove         = false;
        CurrentPlayerLife = PlayerLife;
        CurrentRound      = 1;

        SuddenDeathFlag = true;
        SuddenDeathStartText.SetActive(false);

        // ラウンド進行を受信時にラウンド数更新
        NetworkManager.OnReceived
                      ?.Where(x =>
                      {
                          if (!(x is RoundUpdateRequest res)) return false;

                          return res.Type.Equals("RoundUpdate") && !res.IsReadyAttackedRival;
                      })
                      .Subscribe(_ => CurrentRound++)
                      .AddTo(this);
    }

    private void OnReceived()
    {
        SuddenDeathFlag = true;
    }
	
	private void Update()
	{
        if (SuddenDeathFlag == true)
        {
            //サドンデス開始用テキスト表示
            SuddenDeathStartText.SetActive(true);

            //外壁の縮小
            Transform sotoWallTransform = sotowall.transform;
            Vector3 sotoWallScale = sotoWallTransform.localScale;
            sotoWallScale.x -= WallSpeed * Time.deltaTime;
            sotoWallScale.z -= WallSpeed * Time.deltaTime;
            sotoWallTransform.localScale = sotoWallScale;

            
            // カウントダウン
            CountDown -= Time.deltaTime;

            if (CountDown <= -3)
            {
                SuddenDeathStartText.SetActive(false);
            }
            /*
            if (CountDown <= WallCount)
            {
                WallSpeed = WallSpeedReset;

                SuddenDeathFlag = false;

                CountDown = CountDownReset;
            }
            */
        }
	}

    public static void HitVerification()
    {
        // プレイヤーのライフを1減らす
        CurrentPlayerLife--;

        RoundMove = true;
        SuddenDeathFlag = false;

        // プレイヤーのライフが0になったらリザルトへ
        if (CurrentPlayerLife == 0)
        {
            //リザルトへ
            Debug.Log("Result");
        }
    }
}
