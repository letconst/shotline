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

    private void Start()
    {
        preslider.fillAmount = 1;
        slider.fillAmount = 1;
        AbleDraw = true;
        holdAmount = 0;
        _isHold = false;
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

        if (ShotLineDrawer.currentDis < 1)
        {
            //��������dis���͈͓���������true��Ԃ��ishotlinedrawer�ŕϐ��ցj
            if (Instance.preslider.fillAmount > dis / Instance.MaxLinePower)
            {
                //������dis���͈͓��̏ꍇ
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

    //�Q�[�W��
    public static void HealGauge()
    {
        ////slider��0�̂Ƃ�_isHold��false�ɂ���
        if (Instance.slider.fillAmount == 0)
        {
            _isHold = false;
        }

        //slider�̉񕜏���
        if (Instance.slider.fillAmount < 1 && !ShotLineDrawer.DrawingData.Renderer.enabled)
        {
            Instance.slider.fillAmount += Instance.HealingGauge;
        }

        //preslider�̉񕜏���
        if (Instance.preslider.fillAmount < 1 && !ShotLineDrawer.DrawingData.Renderer.enabled)
        {
            Instance.preslider.fillAmount += Instance.HealingGauge;
        }

        if (Instance.preslider.fillAmount > 0)
        {
            AbleDraw = true;
        }
    }


    //�ˌ����s��ꂽ��{�Q�[�W�����炷����
    //�r�b�O�o���b�g�ł̎ˌ����s��ꂽ��{�Q�[�W�����炷����
    public static void Clicked()
    {
        _isHold = true;
        holdAmount = 0;
    }

    void DealSlider()
    {
        //slider���ǂ��܂Ō��炷��
        if (Instance.slider.fillAmount <= Instance.preslider.fillAmount)
        {
            _isHold = false;
        }

        //slider��preslider�̏���ʕ�����
        if (_isHold)
        {
            ShotLineDrawer.currentDis = 0;
            Instance.slider.fillAmount -= DealSliderSpeed;
        }
    }

}
