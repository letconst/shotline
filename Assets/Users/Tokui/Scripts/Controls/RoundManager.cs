using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class RoundManager : SingletonMonoBehaviour<RoundManager>
{
    [SerializeField]
    private int PlayerLife = 3; //プレイヤーのライフ

    [SerializeField]
    private Text roundText;

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

    public static void HitVerification()
    {
        // プレイヤーのライフを1減らす
        CurrentPlayerLife--;

        RoundMove = true;

        // プレイヤーのライフが0になったらリザルトへ
        if (CurrentPlayerLife == 0)
        {
            //リザルトへ
            Debug.Log("Result");
        }
    }
}
