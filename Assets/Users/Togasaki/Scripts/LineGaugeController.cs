using UnityEngine;
using UnityEngine.UI;

public class LineGaugeController : SingletonMonoBehaviour<LineGaugeController>, IManagedMethod
{
    [SerializeField, Header("予告ゲージ")]
    public Image preslider;

    [SerializeField, Header("本ゲージ")]
    public Image slider;

    [SerializeField, Header("射線ゲージ最大量"), Range(0, 100)]
    public float MaxLinePower = 100;

    [SerializeField, Header("回復スピード")]
    private float HealingGauge = 0.001f;

    [SerializeField,Header("本ゲージの消費スピード"),Range(0.0001f,0.8f)]
    private float DealSliderSpeed = 0.0001f;

    [SerializeField, Header("プレイヤーへの追従時のオフセット値")]
    private Vector3 followOffset;

    //予告ゲージの量
    public static float holdAmount;

    //予告ゲージの量をホールドするかどうか
    private static bool _isHold;

    //ラインをひけるかどうか
    public static bool AbleDraw;

    //回復状態かどうか
    public static bool _isHeal;

    // プレイヤーのTransform
    private Transform _playerTrf;

    public void ManagedStart()
    {
        preslider.fillAmount     = 1;
        slider.fillAmount        = 1;
        AbleDraw                 = true;
        holdAmount               = 0;
        _isHold                  = false;
        LinearDraw._linearDrawOn = true;
        LinearDraw._isLinearDraw = false;
        _isHeal                  = true;
        
        _playerTrf = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void ManagedUpdate()
    {
        HealGauge();
        DealSlider();
        FollowToPlayer();
    }

    //描けるかどうかを返す
    public static bool LineGauge(float dis, ref float rdis)
    {
        bool result = true;

        if (ShotLineDrawer.currentDis < 1)
        {
            //もし引数disが範囲内だったらtrueを返す（shotlinedrawerで変数へ）
            if (Instance.preslider.fillAmount > dis / Instance.MaxLinePower)
            {
                //ここにdisが範囲内の場合
                result = true;
                AbleDraw = true;
                if (dis < 0) dis = 0;
                Instance.preslider.fillAmount -= dis / Instance.MaxLinePower;
            }
            else
            {
                AbleDraw = false;
                result = false;
                if (Instance.preslider.fillAmount < (dis / Instance.MaxLinePower))
                {
                    rdis = Instance.preslider.fillAmount * Instance.MaxLinePower;
                }
                else
                {
                    rdis = Instance.preslider.fillAmount - (dis / Instance.MaxLinePower);
                }

                if (rdis < 0)
                {
                    rdis = 0;
                    Instance.preslider.fillAmount = 0;
                }
                Instance.preslider.fillAmount -= rdis / Instance.MaxLinePower;
            }
        }
        else
        {
            result = false;
        }
        return result;
    }

    //ゲージ回復
    public static void HealGauge()
    {
        if (_isHeal)
        {
            ////sliderが0のとき_isHoldをfalseにする
            if (Instance.slider.fillAmount == 0)
            {
                _isHold = false;
            }

            //sliderの回復処理
            if (Instance.slider.fillAmount < 1)
            {
                Instance.slider.fillAmount += Instance.HealingGauge;
            }

            //presliderの回復処理
            if (Instance.preslider.fillAmount < (1 - ShotLineDrawer.currentDis))
            {
                Instance.preslider.fillAmount += Instance.HealingGauge;
            }

            if (Instance.preslider.fillAmount > 0)
            {
                AbleDraw = true;
            }
        }
    }


    //射撃が行われたら本ゲージを減らす処理
    //ビッグバレットでの射撃が行われたら本ゲージを減らす処理
    public static void Clicked()
    {
        _isHold = true;
        holdAmount = 0;
    
        _isHold = false;
        Instance.slider.fillAmount -= ShotLineDrawer.currentDis;
        Instance.preslider.fillAmount = Instance.slider.fillAmount;
        ShotLineDrawer.currentDis = 0;


    }

    void DealSlider()
    {
        //sliderをどこまで減らすか
        //if (_isHeal && Instance.slider.fillAmount < (1 - ShotLineDrawer.currentDis))
        //{
        //    _isHold = false;
        //    ShotLineDrawer.currentDis = 0;
        //    Instance.slider.fillAmount -= ShotLineDrawer.currentDis;

        //}

        //sliderをpresliderの消費量分消す
        //if (_isHold)
        //{
        //    ShotLineDrawer.currentDis = 0;
        //    Instance.slider.fillAmount -= ShotLineDrawer.currentDis;
        //}
    }

    /// <summary>
    /// ゲージUIの位置をプレイヤーに追従させる
    /// </summary>
    private void FollowToPlayer()
    {
        transform.position = _playerTrf.position + followOffset;
    }
}
