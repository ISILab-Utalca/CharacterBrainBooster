using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CBB.ExternalTool
{
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

        public void SetCurve(Curve curve, float value)
        {
            this.chartPainter.SetCurve(curve, value);
        }

        public void SetCurves((Curve, float)[] curves)
        {
            this.chartPainter.SetCurves(curves);
        }
    }
}
