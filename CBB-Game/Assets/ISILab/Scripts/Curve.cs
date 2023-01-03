using CBB.Api;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Curvas CBB blabla
/// </summary>
[System.Serializable]
public abstract class Curve
{
    [JsonRequired]
    public bool Inverted = false;
    
    public abstract float Calc(params float[] parms);
    public abstract float Calc(float v);

    public static List<Curve> GetCurves()
    {
        IEnumerable<Curve> exporters = typeof(Curve)
        .Assembly.GetTypes()
        .Where(t => t.IsSubclassOf(typeof(Curve)) && !t.IsAbstract)
        .Select(t => (Curve)Activator.CreateInstance(t));

        return exporters.ToList();
    }

    public static List<Vector2> CalcPoints(Curve curve, int steps)
    {
        var points = new List<Vector2>();
        for (int i = 0; i < steps +1; i++)
        {
            var x = (1f / (float)steps) * i;
            var y = curve.Calc(x);
            points.Add(new Vector2(x, y));
        }
        return points;
    }
}

[Curve(name:"Linear")]
[System.Serializable]
public class Linear : Curve
{
    [JsonRequired]
    public float value = 0f;
    [JsonRequired, Param("Slope",0,10)]
    public float m = 1f;
    [JsonRequired, Param("Dx",-1,1)]
    public float dx = 0f;
    [JsonRequired, Param("Dy",-1,1)]
    public float dy = 0f;

    public Linear() { }

    public Linear(float value, float m = 1f, float dx = 0f, float dy = 0f)
    {
        this.value = value;
        this.m = m;
        this.dx = dx;
        this.dy = dy;
    }

    public override float Calc(params float[] parms)
    {
        var value = parms[0];
        var m = parms[1];
        var dx = parms[2];
        var dy = parms[3];

        return Calc(value);
    }

    public override float Calc(float v)
    {
        value = v;
        var toR = Mathf.Clamp01((m * (value + dx)) - dy);
        return Inverted? 1 - toR : toR;
    }

    public override int GetHashCode()
    {
        return (int)(Utils.StringToInt(GetType().ToString()) + value * 10 + m * 100 + dx * 1000 + dy * 10000);
    }
}

[Curve(name:"Inverted exponential")]
[System.Serializable]
public class ExponencialInvertida : Curve
{
    [JsonRequired]
    public float value = 0f;   // X
    [JsonRequired, Param("e", 0, 10)]
    public float e = 2f;       // 2f
    [JsonRequired, Param("Dx", -10, 10)]
    public float dx = 1f;      // 0.0f
    [JsonRequired, Param("Dy", -1, 1)]
    public float dy = 0f;      // 0.0f
    [JsonRequired, Param("Sx", 0, 100)]
    public float sx = 1f;      // 1.0f
    [JsonRequired, Param("Sy", 0, 1)]
    public float sy = 1f;      // 1.0f

    public ExponencialInvertida() { }

    public ExponencialInvertida(float value, float e = 2f, float dx = 1f, float dy = 0f, float sx = 1f, float sy = 1f)
    {
        this.value = value;
        this.e = e;
        this.dx = dx;
        this.dy = dy;
        this.sx = sx;
        this.sy = sy;
    }

    public override float Calc(params float[] parms)
    {
        var value = parms[0];   // X
        var e = parms[1];       // 2f
        var dx = parms[2];      // 0.0f
        var dy = parms[3];      // 0.0f

        return Calc(value);
    }

    public override float Calc(float v)
    {
        value = v;
        var toR = Mathf.Clamp01((Mathf.Log((value * sx) + dx, e) * sy) + dy);
        return Inverted ? 1 - toR : toR;
    }

    public override int GetHashCode()
    {
        return (int)(Utils.StringToInt(GetType().ToString()) + value * 10 + e * 100 + dx * 1000 + dy * 10000);
    }
}

[Curve(name: "Exponential")]
[System.Serializable]
public class Exponencial : Curve
{
    [JsonRequired]
    public float value = 0f;   // X
    [JsonRequired, Param("e", 0, 10)]
    public float e = 2f;       // 2f
    [JsonRequired, Param("Dx", -1, 1)]
    public float dx = 0f;      // 0.0f
    [JsonRequired, Param("Dy", -1, 1)]
    public float dy = 0f;      // 0.0f
    [JsonRequired, Param("Sx", 0, 2)]
    public float sx = 1f;      // 1.0f
    [JsonRequired, Param("Sy", 0, 2)]
    public float sy = 1f;      // 1.0f

    public Exponencial() { }

    public Exponencial(float value, float e = 2f, float dx = 0f, float dy = 0f, float sx = 1f, float sy = 1f)
    {
        this.value = value;
        this.e = e;
        this.dx = dx;
        this.dy = dy;
        this.sx = sx;
        this.sy = sy;
    }

    public override float Calc(params float[] parms)
    {
        var value = parms[0];   // X
        var e = parms[1];       // 2f
        var dx = parms[2];      // 0.0f
        var dy = parms[3];      // 0.0f

        return Calc(value);
    }

    public override float Calc(float v)
    {
        value = v;
        var toR = Mathf.Clamp01((Mathf.Pow((value * sx) + dx, e) * sy) - dy);
        return Inverted ? 1 - toR : toR;
    }

    public override int GetHashCode()
    {
        return (int)(Utils.StringToInt(GetType().ToString()) + value * 10 + e * 100 + dx * 1000 + dy * 10000 + sx * 100000 + sy * 1000000);
    }
}

[Curve(name:"Staggered")]
[System.Serializable]
public class Staggered : Curve
{
    [JsonRequired]
    public float value = 0f;   // X
    [JsonRequired, Param("e", 0, 1)]
    public float e = 0.5f;     // 0.5f
    [JsonRequired, Param("Max", 0, 1)]
    public float max = 0.95f;  // 0.95f;
    [JsonRequired, Param("Min", 0, 1)]
    public float min = 0.05f;  //  0.05f; 

    public Staggered() { }

    public Staggered(float value, float e = 0.5f, float max = 0.95f, float min = 0.05f)
    {
        this.value = value;
        this.e = e;
        this.max = max;
        this.min = min;
    }

    public override float Calc(params float[] parms)
    {
        var value = parms[0];   // X
        var e = parms[1];       // 0.5f
        var max = parms[2];     // 1f
        var min = parms[3];     // 0.1f

        return Calc(value);
    }

    public override float Calc(float v)
    {
        value = v;
        var toR = value >= e ? max : min;
        return Inverted ? 1 - toR : toR;
    }

    public override int GetHashCode()
    {
        return (int)(Utils.StringToInt(GetType().ToString()) + value * 10 + e * 100 + max * 1000 + min * 10000);
    }
}

[Curve(name:"Sigmoide")] // ,"Euler")]
[System.Serializable]
public class Sigmoide : Curve
{
    [JsonRequired]
    public float value = 0f;   // X
    [JsonRequired, Param("Dx", 0, 10)]
    public float dx = 5f;      // 5.0f
    [JsonRequired, Param("Sx", 5, 15)]
    public float sx = 10f;     // 10.0f
    [JsonRequired, Param("Dy", -1, 1)]
    public float dy = 0f;      // 0.0f
    [JsonRequired, Param("Sy", 0, 2)]
    public float sy = 1f;     // 10.0f

    public Sigmoide() { }

    public Sigmoide(float value, float dx = 5f, float sx = 10f, float dy = 0f, float sy = 1f)
    {
        this.value = value;
        this.dx = dx;
        this.sx = sx;
        this.dy = dy;
        this.sy = sy;
    }

    public override float Calc(params float[] parms)
    {
        value = parms[0];   // X
        dx = parms[1];      // 0.0f
        sx = parms[2];
        dy = parms[3];      // 0.0f
        sy = parms[4];

        return Calc(value);
    }

    public override float Calc(float v)
    {
        value = v;
        var toR = Mathf.Clamp01(((1 / (1 + Mathf.Exp(-(value * sx) + dx))) * sy) + dy);
        return Inverted ? 1 - toR : toR;
    }

    public override int GetHashCode()
    {
        return (int)(Utils.StringToInt(GetType().ToString()) + value * 10 + dx * 100 + dy * 1000 + sx * 10000 + sy * 100000);
    }
}

[Curve(name: "Constant")]
[System.Serializable]
public class Constant : Curve
{
    [JsonRequired, Param("Value", 0, 1)]
    public float value = 0.5f;   // X

    public Constant() { }

    public Constant(float value)
    {
        this.value = value;
    }

    public override float Calc(params float[] parms)
    {
        //value = parms[0];
        return Calc(value);
    }

    public override float Calc(float v)
    {
        var toR = Mathf.Clamp01(value);
        return Inverted ? 1 - toR : toR;
    }
}

[Curve(name: "Bell")]
[System.Serializable]
public class Bell : Curve
{
    [JsonRequired]
    public float value;     // X
    [JsonRequired, Param("Base", 1f, 10f)]
    public float b = 10f;
    [JsonRequired, Param("U", 0f, 3f)]
    public float u = 1f;
    [JsonRequired, Param("Dx", 0f, 1f)]
    public float dx = 0.5f;
    [JsonRequired, Param("Dy", -1f, 1f)]
    public float dy = 0f;

    public Bell() { }

    public Bell(float value, float o = 1f, float u = 1f, float dx = 0f, float dy = 0f)
    {
        this.value = value;
        this.b = o;
        this.u = u;
        this.dx = dx;
        this.dy = dy;
    }

    public override float Calc(params float[] parms)
    {
        this.value = parms[0];
        this.b = parms[1];
        this.u = parms[2];
        this.dx = parms[3];
        this.dy = parms[4];

        return Calc(value);
    }

    public override float Calc(float v)
    {
        var toR = Mathf.Clamp01((Mathf.Pow(b, -Mathf.Pow(v - dx, 2) * Mathf.Pow(10, u))) + dy);
        return Inverted ? 1 - toR : toR;
        //return (1f / b * Mathf.Sqrt(2 * Mathf.PI)) * Mathf.Exp(-(0.5f) * ((Mathf.Pow(value - u, 2) / Mathf.Pow(b, 2)))) + dy;
    }
}

