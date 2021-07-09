using UnityEngine;
using UnityEngine.UI;

public class LineGaugeController : MonoBehaviour
{

    [SerializeField, Header("�\���Q�[�W")]
    private Slider preslider;

    [SerializeField, Header("�{�Q�[�W")]
    private Slider slider;

    [SerializeField, Header("�ː��Q�[�W�ő��"), Range(0, 100)]
    private float MaxLinePower = 100;

    [SerializeField, Header("�񕜃X�s�[�h")]
    private float HealingGauge = 0.001f;

    //�g�p�������C���p���[
    private float usedLinePower;

    //�{�Q�[�W�̌��ݗ�
    private float LinePower;

    //���C�����Ђ��邩�ǂ���
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

        //�ː����Ђ����Ƃ��ɗ\���Q�[�W�����炷����
        if (ShotLineDrawer.DrawingData != null && ShotLineDrawer.DrawingData.Renderer.enabled && preslider.value > 0 && !Projectile.One && !BigBullet.ClickBB)
        {
            if (LinePower <= ShotLineUtil.GetFingerPositions(ShotLineDrawer.DrawingData).Length)
            {
                //���C���������Ȃ�����
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
            //�Q�[�W��
            if (slider.value < 1)
            {
                slider.value += HealingGauge;
                preslider.value += HealingGauge;
            }
        }

        //�ˌ����s��ꂽ��{�Q�[�W�����炷����
        if (Projectile.One)
        {
            slider.value = usedLinePower;
        }

        //�r�b�O�o���b�g�ł̎ˌ����s��ꂽ��{�Q�[�W�����炷����
        if (BigBullet.ClickBB)
        {

        }

    }


}
