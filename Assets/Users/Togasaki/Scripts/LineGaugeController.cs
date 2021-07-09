using UnityEngine;
using UnityEngine.UI;

public class LineGaugeController : SingletonMonoBehaviour<LineGaugeController>
{
    [SerializeField, Header("�\���Q�[�W")]
    private Image preslider;

    [SerializeField, Header("�{�Q�[�W")]
    private Image slider;

    [SerializeField, Header("�ː��Q�[�W�ő��"), Range(0, 100)]
    private float MaxLinePower = 100;

    [SerializeField, Header("�񕜃X�s�[�h")]
    private float HealingGauge = 0.001f;

    //���C�����Ђ��邩�ǂ���
    public static bool AbleDraw;


    private void Start()
    {
        preslider.fillAmount = 1;
        slider.fillAmount = 1;
        AbleDraw = true;
    }


    //�`���邩�ǂ�����Ԃ�
    public static bool LineGauge(float dis, ref float rdis)
    {

        bool result = true;

        //�����������͈͓���������true��Ԃ��ishotlinedrawer�ŕϐ��ցj
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

        ////�ː����Ђ����Ƃ��ɗ\���Q�[�W�����炷����
        //if (ShotLineDrawer.DrawingData != null && ShotLineDrawer.DrawingData.Renderer.enabled && !Projectile.One && !BigBullet.ClickBB)
        //{

        //    if (Instance.preslider.fillAmount <= dis)
        //    {
        //        //���C���������Ȃ�����
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
        //    //�Q�[�W��
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

    //�Q�[�W��
    public static void HealGauge()
    {
        //���݂������ʂ�܂���I�I
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


    //�ˌ����s��ꂽ��{�Q�[�W�����炷����
    //�r�b�O�o���b�g�ł̎ˌ����s��ꂽ��{�Q�[�W�����炷����
    public static void Clicked()
    {
        LineGaugeController.Instance.slider.fillAmount = LineGaugeController.Instance.preslider.fillAmount;
    }


}
