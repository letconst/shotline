using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public enum JudgeType
{
    Damage,
    Hit,
}

public enum ResultType
{
    Lose,
    Win,
}

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

    private float CountDown;

    public bool SuddenDeathFlag;

    public bool PlayerDeathFlag;

    // 現在ラウンドが切り替わっている最中かどうか判別
    public static bool RoundMove = false;

    public  int  playerPoint;
    private int  _rivalPoint;
    private bool _isFadeInSuddenDeathImg;

    private static Sprite[] _roundTitleSprites;
    private static Sprite[] _battleResultSprites;
    private static Sprite[] _judgeSprites;
    private static Sprite[] _p1PointSprites;
    private static Sprite[] _p2PointSprites;

    public static int CurrentPlayerLife { get; private set; }
    public static int CurrentRound      { get; private set; }

    private const string RoundUIBasePath = "Sprites/UI/ROUND";

    protected void Start()
    {
        RoundMove         = false;
        CurrentPlayerLife = PlayerLife;
        CurrentRound      = 1;

        // 各種UI画像読み込み
        _roundTitleSprites   = Resources.LoadAll<Sprite>($"{RoundUIBasePath}/Title");
        _battleResultSprites = Resources.LoadAll<Sprite>($"{RoundUIBasePath}/Result");
        _judgeSprites        = Resources.LoadAll<Sprite>($"{RoundUIBasePath}/Judge");
        _p1PointSprites      = Resources.LoadAll<Sprite>($"{RoundUIBasePath}/P1Point");
        _p2PointSprites      = Resources.LoadAll<Sprite>($"{RoundUIBasePath}/P2Point");

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
                FadeTransition.FadeOut(MainGameProperty.SuddenDeathImg, .5f);
            }

            if (CountDown >= WallCount)
            {
                SuddenDeathFlag = false;

                CountDown = CountDownReset;
            }
        }
    }

    public static void HitVerification()
    {
        // プレイヤーのライフを1減らす
        CurrentPlayerLife--;
        Instance._rivalPoint++;

        Instance.SuddenDeathFlag = false;

        // プレイヤーのライフが0になったらリザルトへ
        if (CurrentPlayerLife == 0)
        {
            //リザルトへ
            Debug.Log("Result");
        }
    }

    /// <summary>
    /// ステージ参加後のフェード処理
    /// </summary>
    public static async UniTask RoundInitFade()
    {
        MainGameProperty.RoundTitleImg.sprite = _roundTitleSprites[CurrentRound - 1];

        UniTask titleFade = FadeTransition.FadeIn(MainGameProperty.RoundTitleImg, .5f);
        UniTask pointFade = FadeTransition.FadeIn(MainGameProperty.PlayerPointsCanvasGroup, .5f);

        await UniTask.WhenAll(titleFade, pointFade);
    }

    /// <summary>
    /// ラウンド開始時のフェードアウト処理
    /// </summary>
    public static async UniTask RoundStartFadeOut()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(1));

        UniTask titleFade = FadeTransition.FadeOut(MainGameProperty.RoundTitleImg, .5f);
        UniTask pointFade = FadeTransition.FadeOut(MainGameProperty.PlayerPointsCanvasGroup, .5f);

        await UniTask.WhenAll(titleFade, pointFade);

        Time.timeScale                    = 1;
        MainGameController.isControllable = true;
        PlayerController.isDamaged        = false;
        RoundMove                         = false;
        SystemUIManager.SetInputBlockerVisibility(false);
    }

    /// <summary>
    /// 決着時のフェードアウト処理
    /// </summary>
    /// <param name="judgeType">表示する判定結果</param>
    public static async UniTask RoundUpdateFadeOut(JudgeType judgeType)
    {
        if (RoundMove) return;

        RoundMove                               = true;
        Time.timeScale                          = .1f;
        MainGameController.isControllable       = false;
        MainGameProperty.BattleResultImg.sprite = _judgeSprites[(int) judgeType];
        SystemUIManager.SetInputBlockerVisibility(true);

        await FadeTransition.FadeIn(MainGameProperty.BattleResultImg, .1f);
        await UniTask.Delay(TimeSpan.FromSeconds(1), true);
        await FadeTransition.FadeIn(SystemProperty.FadeCanvasGroup, .5f);

        if (NetworkManager.IsOwner)
        {
            MainGameProperty.P1PointImg.sprite = _p1PointSprites[Instance.playerPoint];
            MainGameProperty.P2PointImg.sprite = _p2PointSprites[Instance._rivalPoint];
        }
        else
        {
            MainGameProperty.P1PointImg.sprite = _p1PointSprites[Instance._rivalPoint];
            MainGameProperty.P2PointImg.sprite = _p2PointSprites[Instance.playerPoint];
        }

        MainGameProperty.BattleResultImg.color         = new Color(1, 1, 1, 0);
        MainGameProperty.PlayerPointsCanvasGroup.alpha = 1;
    }

    /// <summary>
    /// 決着後（ラウンド移行後）のフェードイン処理
    /// </summary>
    public static async UniTask RoundUpdateFadeIn()
    {
        if (!RoundMove) return;

        Time.timeScale                         = 1;
        MainGameProperty.BattleResultImg.color = new Color(1, 1, 1, 0);
        MainGameProperty.RoundTitleImg.sprite  = _roundTitleSprites[CurrentRound - 1];
        MainGameProperty.RoundTitleImg.color   = Color.white;

        await FadeTransition.FadeOut(SystemProperty.FadeCanvasGroup, .5f);
    }

    /// <summary>
    /// バトル結果を表示する
    /// </summary>
    /// <param name="resultType">表示する結果</param>
    public static async UniTask ShowBattleResult(ResultType resultType)
    {
        Time.timeScale                          = .1f;
        MainGameController.isControllable       = false;
        MainGameProperty.BattleResultImg.sprite = _battleResultSprites[(int) resultType];

        SystemUIManager.SetInputBlockerVisibility(true);

        // タップでタイトルに戻れる旨を表示
        SystemUIManager.ShowStatusText(StatusText.TapToTitle, false);
        MainGameController.isChangeableSceneToTitle = true;

        await FadeTransition.FadeIn(MainGameProperty.BattleResultImg, .1f);
    }

    public void ResetCount()
    {
        CountDown = CountDownReset;
    }

    private void OnReceived(object res)
    {
        var @base = (InRoomRequestBase) res;
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
                FadeTransition.FadeIn(MainGameProperty.SuddenDeathImg, .5f);

                break;
            }
        }
    }
}
