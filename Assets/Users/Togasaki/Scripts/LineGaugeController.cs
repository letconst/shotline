using UnityEngine;
using UnityEngine.UI;

public class LineGaugeController : SingletonMonoBehaviour<LineGaugeController>
{
    [SerializeField, Header("予告ゲージ")]
    private Image preslider;

    [SerializeField, Header("本ゲージ")]
    private Image slider;

    [SerializeField, Header("射線ゲージ最大量"), Range(0, 100)]
    private float MaxLinePower = 100;

    [SerializeField, Header("回復スピード")]
    private float HealingGauge = 0.001f;

    //使用したラインパワー
    private float usedLinePower;

    //ラインをひけるかどうか
    public static bool AbleDraw;

    //予告ゲージ保管用
    private float prefl;

    private void Start()
    {
        usedLinePower = 1;
        preslider.fillAmount = 1;
        slider.fillAmount = 1;
        AbleDraw = true;
        prefl = MaxLinePower;
    }

    private void Update()
    {
        LineGauge();
    }


    private void LineGauge()
    {
        //射線をひいたときに予告ゲージを減らす処理
        if (ShotLineDrawer.DrawingData != null && ShotLineDrawer.DrawingData.Renderer.enabled && !Projectile.One && !BigBullet.ClickBB)
        {

            if (prefl <= ShotLineUtil.GetFingerPositions(ShotLineDrawer.DrawingData).Length)
            {
                //ラインを引けなくする
                AbleDraw = false;
                usedLinePower = 0;
            }
            else if (AbleDraw)
            {
                usedLinePower = (prefl - ShotLineUtil.GetFingerPositions(ShotLineDrawer.DrawingData).Length) / MaxLinePower;
                preslider.fillAmount = usedLinePower;

            }

        }
        else if (preslider.fillAmount < 1)
        {
            //ゲージ回復
            slider.fillAmount += HealingGauge;
            preslider.fillAmount += HealingGauge;
            prefl = (preslider.fillAmount * 100);

            AbleDraw = true;

            if (preslider.fillAmount == 1)
            {
                gameObject.SetActive(false);
            }


        }

    }


    //射撃が行われたら本ゲージを減らす処理
    //ビッグバレットでの射撃が行われたら本ゲージを減らす処理
    public static void Clicked()
    {
        LineGaugeController.Instance.slider.fillAmount = LineGaugeController.Instance.usedLinePower;
        LineGaugeController.Instance.prefl = LineGaugeController.Instance.slider.fillAmount;
    }


}
