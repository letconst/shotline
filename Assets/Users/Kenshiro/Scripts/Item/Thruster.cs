using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thruster : ActiveItem
{
    GameObject Player;
    CharaMove Chara;

    CharaMove BoostSpeed;

    //移動速度カウントダウン用変数
    public float MoveCountDown;
    bool ClickButton;

    int seconds;

    //無敵時間カウントダウン用変数
    public float FlagCountDown;

    protected override void Init()
    {
        base.Init();

        //スラスターの所持確認
        Player = GameObject.FindGameObjectWithTag("Player");
        Chara = Player.GetComponent<CharaMove>();

    }

    protected override void OnClickButton()
    {
        //ボタンを押したとき移動速度1.5倍
        Chara.speedRatio = 1.5f;

        //ボタンを押したら弾のフラグ処理を true にする
        Chara.Muteki = true;

        //ボタンを押したときカウント開始
        ClickButton = true;

        ClearItemIcon();
    }

    protected override void UpdateFunction()
    {

        //移動速度1.5倍カウント
        if (ClickButton && MoveCountDown > 0)
        {
            MoveCountDown -= Time.deltaTime;
            seconds = (int)MoveCountDown;
        }

        //無敵時間のカウント
        if (Chara.Muteki == true)
        {
            FlagCountDown -= Time.deltaTime;
            seconds = (int)MoveCountDown;

            //true の時、向いている方向に移動
            BoostSpeed = Player.GetComponent<CharaMove>();

            BoostSpeed.moveX = BoostSpeed.CurrentSpeed;
            BoostSpeed.moveZ = BoostSpeed.CurrentSpeed;
        }

        //移動速度を元に戻す
        if (MoveCountDown <= 0)
        {
            Chara.speedRatio = 1f;
        }

        //無敵解除
        //フラグを false に戻す
        if (FlagCountDown <= 0)
        {
            Chara.Muteki = false;
        }

        //両カウントが0になったら Terminate を呼ぶ
        if (MoveCountDown <= 0 && FlagCountDown <= 0)
        {
            Terminate();
        }
    }

    public override void Terminate()
    {
        base.Terminate();
    }
}
