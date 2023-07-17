using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Chart : VisualElement
{
    public new class UxmlFactory : UxmlFactory<Chart, UxmlTraits> { }

    private PainterChart2D chartPainter;

    public Chart()
    {
        var vt = Resources.Load<VisualTreeAsset>("Chart");
        vt.CloneTree(this);

        this.chartPainter = this.Q<PainterChart2D>();
    }

    public void SetCurve(Curve curve)
    {
        this.chartPainter.SetCurve(curve);
    }
}
