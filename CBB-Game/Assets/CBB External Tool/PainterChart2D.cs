using System.Collections.Generic;
using System.Drawing;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UIElements;
using Color = UnityEngine.Color;

public struct CurveFormat
{
    public Curve curve;
    public float value;
    public Color color;
    public bool showValue; 
}

public class PainterChart2D : VisualElement
{
    public new class UxmlFactory : UxmlFactory<PainterChart2D, UxmlTraits> { }

    private Painter2D paint2D;
    private CurveFormat[] curvesFormats = new CurveFormat[] { new CurveFormat() { curve = new ExponencialInvertida(), value = 0.5f, color = Color.green, showValue = true } };

    private Color colorLine = new(1f, 1f, 1f, .7f);
    private Color colorGrid = new(0f, 0f, 0f, .05f);
    //private Color colorCurve = Color.green;
    private Color colorValuePoint = Color.red;
    private Color colorValueLine = new(1f, 0f, 0f, .2f);

    public float Height => this.resolvedStyle.height;
    public float Width => this.resolvedStyle.width;

    public PainterChart2D()
    {
        this.style.borderRightWidth = this.style.borderTopWidth = 0;
        this.style.borderBottomWidth = this.style.borderLeftWidth = 2;
        this.style.borderBottomColor = this.style.borderLeftColor = this.style.borderRightColor = this.style.borderTopColor = colorLine;

        //this.style.width = 300; // (!) deshardcodear
        //this.style.height = 160;

        this.generateVisualContent += OnGenerateVisualContent;
    }

    void OnGenerateVisualContent(MeshGenerationContext mgc)
    {
        // Init
        paint2D = mgc.painter2D;

        // Draw backgorund
        DrawBackground(colorLine, colorGrid);

        foreach (var curve in curvesFormats)
        {
            // Draw curve
            var points01 = Curve.CalcPoints(curve.curve, 50);
            var points = AdjustPoints(points01);
            DrawLine(points, curve.color);

            // Check if show value point
            if (!curve.showValue)
                continue;

            // Draw value point
            var x = curve.value;
            var y = curve.curve.Calc(x);
            var point = AdjustPoint(new Vector2(x, y));
            DrawPoint(point, 2.0f, colorValuePoint);

            // Draw value lines
            var start = new Vector2(2, point.y);
            var end = new Vector2(point.x, Height - 2);
            DrawLine(new List<Vector2>() { start, point, end }, colorValueLine, 2);
        }
    }

    public void SetCurve(Curve curve)
    {
        this.curvesFormats = new CurveFormat[] { new CurveFormat() { curve = curve, value = 0.5f, color = Color.green, showValue = false } };
        this.MarkDirtyRepaint();
    }

    public void SetCurve(Curve curve, float value,Color color, bool showValue)
    {
        this.curvesFormats = new CurveFormat[] { new CurveFormat() { curve = curve, value = value, color = color, showValue = showValue } };
        this.MarkDirtyRepaint();
    }

    public void SetCurves(CurveFormat[] curves)
    {
        curvesFormats = curves;
        this.MarkDirtyRepaint();
    }

    private Vector2 AdjustPoint(Vector2 point01)
    {
        var h = Height;
        var w = Width;
        var point = new Vector2(point01.x * w, h - point01.y * h) + new Vector2(2, -2);
        return point;
    }

    private List<Vector2> AdjustPoints(List<Vector2> points01)
    {
        var steps = points01.Count;
        var points = new List<Vector2>();
        var h = Height - 2;
        var w = Width - 2;
        for (int i = 0; i < steps; i++)
        {
            var point = new Vector2(points01[i].x * w, Height - points01[i].y * h) + new Vector2(2, -2);
            points.Add(point);
        }
        return points;
    }

    public void DrawBackground(Color colorLine, Color colorGrid)
    {
        var d = (Width - 2) / 9f;
        for (int i = 1; i < 10; i++)
        {
            var start = new Vector2(d * i, 0f);
            var end = new Vector2(d * i, Height - 2);
            DrawLine(new List<Vector2>() { start, end }, colorGrid, 2);
        }

        d = (Height) / 9f;
        for (int i = 0; i < 9; i++)
        {
            var start = new Vector2(2, d * i + 1);
            var end = new Vector2(Width - 1, d * i + 1);
            DrawLine(new List<Vector2>() { start, end }, colorGrid, 2);
        }
    }

    private void DrawLine(List<Vector2> point, Color color, float stroke = 1)
    {
        paint2D.strokeColor = color;
        paint2D.BeginPath();

        paint2D.MoveTo(point[0]);
        for (int i = 1; i < point.Count; i++)
        {
            paint2D.LineTo(point[i]);
        }

        paint2D.lineWidth = stroke;
        paint2D.Stroke();
    }

    private void DrawPoint(Vector2 pos, float radius, Color color)
    {
        paint2D.fillColor = color;
        paint2D.BeginPath();
        paint2D.Arc(pos, radius, 0.0f, 360.0f);
        paint2D.Fill();
    }

}
