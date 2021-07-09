using UnityEngine;
using UnityEngine.UI;

public class LineGaugeController : MonoBehaviour
{

    [SerializeField, Header("予告ゲージ")]
    private Slider preslider;

    [SerializeField, Header("本ゲージ")]
    private Slider slider;

    [SerializeField, Header("射線ゲージ最大量"), Range(0, 100)]
    private float MaxLinePower = 100;

    [SerializeField, Header("回復スピード")]
    private float HealingGauge = 0.001f;

    //使用したラインパワー
    private float usedLinePower;

    //本ゲージの現在量
    private float LinePower;

    //ラインをひけるかどうか
    public static bool AbleDraw;

    private void Start()
    {
        usedLinePower = 1;
        LinePower = MaxLinePower;
        preslider.value = 1;
        slider.value = 1;
        AbleDraw = true;
    }

    private void Update()
    {
        LineGauge();
    }


    private void LineGauge()
    {

        //射線をひいたときに予告ゲージを減らす処理
        if (ShotLineDrawer.DrawingData != null && ShotLineDrawer.DrawingData.Renderer.enabled && preslider.value > 0 && !Projectile.One && !BigBullet.ClickBB)
        {
            if (LinePower <= ShotLineUtil.GetFingerPositions(ShotLineDrawer.DrawingData).Length)
            {
                //ラインを引けなくする
                AbleDraw = false;
                usedLinePower = 0;
                preslider.value = 0;
                LinePower = 0;
            }
            else if(AbleDraw)
            {
                usedLinePower = (LinePower - ShotLineUtil.GetFingerPositions(ShotLineDrawer.DrawingData).Length) / MaxLinePower;
                preslider.value = usedLinePower;
                
            }

        }
        else
        {
            //ゲージ回復
            if (slider.value < 1)
            {
                slider.value += HealingGauge;
                preslider.value += HealingGauge;
            }
        }

        //射撃が行われたら本ゲージを減らす処理
        if (Projectile.One)
        {
            slider.value = usedLinePower;
        }

        //ビッグバレットでの射撃が行われたら本ゲージを減らす処理
        if (BigBullet.ClickBB)
        {

        }

    }


}
