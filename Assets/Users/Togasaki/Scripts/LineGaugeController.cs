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

    //�g�p�������C���p���[
    private float usedLinePower;

    //���C�����Ђ��邩�ǂ���
    public static bool AbleDraw;

    //�\���Q�[�W�ۊǗp
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
        //�ː����Ђ����Ƃ��ɗ\���Q�[�W�����炷����
        if (ShotLineDrawer.DrawingData != null && ShotLineDrawer.DrawingData.Renderer.enabled && !Projectile.One && !BigBullet.ClickBB)
        {

            if (prefl <= ShotLineUtil.GetFingerPositions(ShotLineDrawer.DrawingData).Length)
            {
                //���C���������Ȃ�����
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
            //�Q�[�W��
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


    //�ˌ����s��ꂽ��{�Q�[�W�����炷����
    //�r�b�O�o���b�g�ł̎ˌ����s��ꂽ��{�Q�[�W�����炷����
    public static void Clicked()
    {
        LineGaugeController.Instance.slider.fillAmount = LineGaugeController.Instance.usedLinePower;
        LineGaugeController.Instance.prefl = LineGaugeController.Instance.slider.fillAmount;
    }


}
