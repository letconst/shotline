using UnityEngine;

public class LinearDraw : ActiveItem
{
    //ライナードローを取得したときのブール
    public static bool _linearDrawOn = true;

    //ライナードローで射線をひくときに最初の一度だけtrueになっているブール
    public static bool _firstDraw = false;

    protected override void Init()
    {
        base.Init();
        _linearDrawOn = true;
        _firstDraw = false;
    }

    public override void Terminate()
    {
        _linearDrawOn = true;
        _firstDraw = false;
        base.Terminate();
    }

    protected override void OnClickButton()
    {
        if (_linearDrawOn)
        {
            _firstDraw = true;
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
