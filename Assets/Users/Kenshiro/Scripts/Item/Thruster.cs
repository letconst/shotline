using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thruster : ActiveItem
{
    GameObject Player;
    CharaMove moveSpeed;

    protected override void Init()
    {
        //スラスターの所持確認
     
        base.Init();
    }

    protected override void OnClickButton()
    {

        //ボタンを押したとき移動速度1.5倍
        Player = GameObject.Find("Player");
        moveSpeed = Player.GetComponent<CharaMove>();
        moveSpeed.speed *= 1.5f;


        //ボタンを押したとき無敵時間のカウントの開始
    }

    protected override void UpdateFunction()
    {

        //移動速度1.5倍カウント


        //無敵時間のカウント


        //無敵時間が過ぎたら Terminate を呼ぶ


    }

    protected override void Terminate()
    {
        //移動速度を元に戻す



        //無敵解除


        base.Terminate();
    }
}
