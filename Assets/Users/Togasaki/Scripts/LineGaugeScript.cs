using UnityEngine;
using UnityEngine.UI;

public class LineGaugeScript : MonoBehaviour
{

    [SerializeField, Header("�\���Q�[�W")]
    private Slider preslider;

    [SerializeField, Header("�{�Q�[�W")]
    private Slider slider;


    [SerializeField, Header("�ː��Q�[�W�ő��")]
    private float MaxLinePower = 100;

    //�g�p�������C���p���[
    private float usedLinePower;

    //�{�X���C�_�[�̌��ݗ�
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

        //�ː����Ђ����Ƃ��ɗ\���Q�[�W�����炷����
        if (ShotLineDrawer.DrawingData != null && ShotLineDrawer.DrawingData.Renderer.enabled && preslider.value > 0)
        {
            usedLinePower = (LinePower - ShotLineUtil.GetFingerPositions(ShotLineDrawer.DrawingData).Length) / MaxLinePower;
            preslider.value = usedLinePower;

        }

        //�ˌ����s��ꂽ��{�Q�[�W�����炷����
        if (Projectile.One)
        {

        }

        //�r�b�O�o���b�g�ł̎ˌ����s��ꂽ��{�Q�[�W�����炷����
        if (BigBullet.BBOn)
        {

        }

    }


}
