using UnityEngine;

public class LinearDraw : ActiveItem
{
    public static bool _linearDrawOn = true;

    protected override void Init()
    {
        base.Init();
        _linearDrawOn = true;
    }

    public override void Terminate()
    {
        _linearDrawOn = true;
        base.Terminate();
    }

    protected override void OnClickButton()
    {
        if (_linearDrawOn)
        {
            NumQuantity.CulLinear(_linearDrawOn);
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
