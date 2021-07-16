using UnityEngine;
using UnityEngine.UI;

public class LineGaugeController : SingletonMonoBehaviour<LineGaugeController>
{
    [SerializeField, Header("�\���Q�[�W")]
    public Image preslider;

    [SerializeField, Header("�{�Q�[�W")]
    public Image slider;

    [SerializeField, Header("�ː��Q�[�W�ő��"), Range(0, 100)]
    public float MaxLinePower = 100;

    [SerializeField, Header("�񕜃X�s�[�h")]
    private float HealingGauge = 0.001f;

    [SerializeField,Header("�{�Q�[�W�̏���X�s�[�h"),Range(0.0001f,0.8f)]
    private float DealSliderSpeed = 0.0001f;

    //�\���Q�[�W�̗�
    public static float holdAmount;

    //�\���Q�[�W�̗ʂ��z�[���h���邩�ǂ���
    private static bool _isHold;

    //���C�����Ђ��邩�ǂ���
    public static bool AbleDraw;

    //preslider���񕜂ł��邩�ǂ���
    public static bool _isHeal;


    private void Start()
    {
        preslider.fillAmount = 1;
        slider.fillAmount = 1;
        AbleDraw = true;
        holdAmount = 0;
        _isHold = false;
        _isHeal = false;
    }

    private void Update()
    {
        HealGauge();
        DealSlider();
    }

    //�`���邩�ǂ�����Ԃ�
    public static bool LineGauge(float dis, ref float rdis)
    {

        bool result = true;

        //��������dis���͈͓���������true��Ԃ��ishotlinedrawer�ŕϐ��ցj
        if (Instance.preslider.fillAmount > 0)
        {
            result = true;
            Instance.preslider.fillAmount -= dis / Instance.MaxLinePower;
            AbleDraw = false;
        }
        else
        {
            result = false;
            rdis = dis - Instance.preslider.fillAmount;
        }

        return result;
    }

    //�Q�[�W��
    public static void HealGauge()
    {
        ////slider��0�̂Ƃ�_isHold��false�ɂ���
        if (Instance.slider.fillAmount == 0)
        {
            _isHold = false;
        }

        //slider�̉񕜏���
        //if (Instance.slider.fillAmount < 1)
        //{
        //    Instance.slider.fillAmount += Instance.HealingGauge;
        //}

        //preslider�̉񕜏���
        if (holdAmount < Instance.preslider.fillAmount || ShotLineDrawer.currentDis == 0)
        {
            Instance.slider.fillAmount += Instance.HealingGauge;
            Instance.preslider.fillAmount += Instance.HealingGauge;
        }
        //if(ShotLineDrawer.currentDis == 0)
        //{
        //    _isHeal = true;
        //}

        //if (_isHeal)
        //{
        //    Instance.preslider.fillAmount += Instance.HealingGauge;
        //}

        if (!(Instance.preslider.fillAmount >= 1) && Instance.preslider.fillAmount > 0)
        {
            AbleDraw = true;
        }
    }


    //�ˌ����s��ꂽ��{�Q�[�W�����炷����
    //�r�b�O�o���b�g�ł̎ˌ����s��ꂽ��{�Q�[�W�����炷����
    public static void Clicked()
    {
        _isHold = true;
        _isHeal = true;
        holdAmount = 0;
    }
    void DealSlider()
    {
        if (Instance.slider.fillAmount <= Instance.preslider.fillAmount)
        {
            _isHold = false;
        }

        if (_isHold)
        {
            ShotLineDrawer.currentDis = 0;
            Instance.slider.fillAmount -= DealSliderSpeed;
        }
    }

}
