using UnityEngine;
using UnityEngine.UI;

public class LineGaugeScript : MonoBehaviour
{

    [SerializeField, Header("予告ゲージ")]
    private Slider preslider;

    [SerializeField, Header("本ゲージ")]
    private Slider slider;


    [SerializeField, Header("射線ゲージ最大量")]
    private float MaxLinePower = 100;

    //使用したラインパワー
    private float usedLinePower;

    //本スライダーの現在量
    private float LinePower;

    private void Start()
    {
        LinePower = MaxLinePower;
        preslider.value = 1;
        slider.value = 1;
    }

    private void Update()
    {
        LineGauge();
    }

    private void LineGauge()
    {

        //射線をひいたときに予告ゲージを減らす処理
        if (ShotLineDrawer.DrawingData != null && ShotLineDrawer.DrawingData.Renderer.enabled && preslider.value > 0)
        {
            usedLinePower = (LinePower - ShotLineUtil.GetFingerPositions(ShotLineDrawer.DrawingData).Length) / MaxLinePower;
            preslider.value = usedLinePower;

        }

        //射撃が行われたら本ゲージを減らす処理
        if (Projectile.One)
        {

        }

        //ビッグバレットでの射撃が行われたら本ゲージを減らす処理
        if (BigBullet.BBOn)
        {

        }

    }


}
