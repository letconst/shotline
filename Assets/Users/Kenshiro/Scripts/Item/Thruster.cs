using UnityEngine;

public class Thruster : ActiveItem
{
    GameObject Player;
    CharaMove Chara;

    CharacterController BoostSpeed;
    Vector3 Direction;
    //移動速度カウントダウン用変数
    public float MoveCountDown;
    bool ClickButton;


    //無敵時間カウントダウン用変数
    public float FlagCountDown;

    protected override void Init()
    {
        base.Init();

        //スラスターの所持確認
        Player = GameObject.FindGameObjectWithTag("Player");
        Chara = Player.GetComponent<CharaMove>();

        //CharacterControllerを取得
        BoostSpeed = Player.GetComponent<CharacterController>();

    }

    protected override void OnClickButton()
    {
        base.OnClickButton();

        //ボタンを押したとき移動速度1.5倍
        Chara.speedRatio = 5f;

        //ボタンを押したら弾のフラグ処理を true にする
        Chara.Thruster = true;

        //ボタンを押したときカウント開始
        ClickButton = true;

        //ボタンを押すとPlayerの向いている方向を取得
        Direction = Player.transform.rotation * new Vector3(0, 0, Chara.CurrentSpeed);

        ClearItemIcon();
    }

    protected override void UpdateFunction()
    {

        //移動速度1.5倍カウント
        if (ClickButton && MoveCountDown > 0)
        {
            MoveCountDown -= Time.deltaTime;
        }

        //無敵時間のカウント
        if (Chara.Thruster == true)
        {
            FlagCountDown -= Time.deltaTime;

            //true の時、向いている方向に移動
            BoostSpeed.SimpleMove(Direction);
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
            Chara.Thruster = false;
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
        Chara.speedRatio = 1;
        Chara.Thruster   = false;
    }
}
