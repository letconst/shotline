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

    //ラインをひけるかどうか
    public static bool AbleDraw;


    private void Start()
    {
        preslider.fillAmount = 1;
        slider.fillAmount = 1;
        AbleDraw = true;
    }


    //描けるかどうかを返す
    public static bool LineGauge(float dis, ref float rdis)
    {

        bool result = true;

        //もし引数が範囲内だったらtrueを返す（shotlinedrawerで変数へ）
        if (Instance.preslider.fillAmount > 0)
        {
            result = true;
            Instance.preslider.fillAmount -= dis / Instance.MaxLinePower;
        }
        else
        {
            result = false;
            rdis = dis - Instance.preslider.fillAmount;
            AbleDraw = false;
        }

        ////射線をひいたときに予告ゲージを減らす処理
        //if (ShotLineDrawer.DrawingData != null && ShotLineDrawer.DrawingData.Renderer.enabled && !Projectile.One && !BigBullet.ClickBB)
        //{

        //    if (Instance.preslider.fillAmount <= dis)
        //    {
        //        //ラインを引けなくする
        //        //AbleDraw = false;
        //        Instance.usedLinePower = 0;
        //    }
        //    else if (AbleDraw)
        //    {
        //        Instance.usedLinePower = (Instance.preslider.fillAmount - dis) / Instance.MaxLinePower;
        //        Instance.preslider.fillAmount = Instance.usedLinePower;

        //    }
        //}
        //else if (Instance.preslider.fillAmount < 1)
        //{
        //    //ゲージ回復
        //    Instance.slider.fillAmount += Instance.HealingGauge;
        //    Instance.preslider.fillAmount += Instance.HealingGauge;
        //    Instance.prefl = (Instance.preslider.fillAmount * 100);

        //    AbleDraw = true;

        //    if (Instance.preslider.fillAmount == 1)
        //    {
        //        Instance.gameObject.SetActive(false);
        //    }

        //}

        return result;

    }

    //ゲージ回復
    public static void HealGauge()
    {
        //現在ここが通りません！！
        if (ShotLineDrawer.DrawingData == null && Instance.preslider.fillAmount < 1)
        {
            Debug.Log("Enter");
            Instance.slider.fillAmount += Instance.HealingGauge;
            Instance.preslider.fillAmount += Instance.HealingGauge;

            AbleDraw = true;

            //if (Instance.preslider.fillAmount == 1)
            //{
            //    Instance.gameObject.SetActive(false);
            //}
        }
    }


    //射撃が行われたら本ゲージを減らす処理
    //ビッグバレットでの射撃が行われたら本ゲージを減らす処理
    public static void Clicked()
    {
        LineGaugeController.Instance.slider.fillAmount = LineGaugeController.Instance.preslider.fillAmount;
    }


}
