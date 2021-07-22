using UnityEngine;

public class LinearDraw : ActiveItem
{
    //���C�i�[�h���[���擾�����Ƃ��̃u�[��
    public static bool _linearDrawOn = true;

    //���C�i�[�h���[�Ŏː����Ђ��Ƃ���true�ɂȂ�u�[��
    public static bool _isLinearDraw = false;

    protected override void Init()
    {
        base.Init();
        _linearDrawOn = true;
        _isLinearDraw = false;
    }

    public override void Terminate()
    {
        _linearDrawOn = true;
        _isLinearDraw = false;
        base.Terminate();
    }

    protected override void OnClickButton()
    {
        if (_linearDrawOn)
        {
            _isLinearDraw = true;
            NumQuantity.CulLinear(_linearDrawOn);
            _linearDrawOn = false;
        }
        else if (!(_linearDrawOn))
        {
            NumQuantity.CulLinear(_linearDrawOn);
            Terminate();
        }
    }

    protected override void UpdateFunction()
    {
    }
}
