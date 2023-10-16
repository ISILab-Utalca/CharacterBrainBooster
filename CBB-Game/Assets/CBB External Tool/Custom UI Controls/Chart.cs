using System.Drawing;
using System.Linq;
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

        public void SetCurve(Curve curve)
        {
            this.chartPainter.SetCurve(curve);
        }

        public void SetCurve(Curve curve, float value)
        {
            this.chartPainter.SetCurve(curve, value, UnityEngine.Color.green, false);
        }

        public void SetCurves((Curve, float)[] curves)
        {
            var x = curves.ToList().Select(c => new CurveFormat() { curve = c.Item1, value = c.Item2, color = UnityEngine.Color.green, showValue = false }).ToArray();

            this.chartPainter.SetCurves(x);
        }
    }
}
