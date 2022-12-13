using CBB.Api;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
public class Escalonada : Curve
{
    [JsonRequired]
    public float value = 0f;   // X
    [JsonRequired, Param("e", 0, 1)]
    public float e = 0.5f;     // 0.5f
    [JsonRequired, Param("Max", 0, 1)]
    public float max = 0.95f;  // 0.95f;
    [JsonRequired, Param("Min", 0, 1)]
    public float min = 0.05f;  //  0.05f; 

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
    [JsonRequired, Param("De", -10, 10)]
    public float de = 0f;      // 0.0f
    [JsonRequired, Param("Dx", 0, 10)]
    public float dx = 5f;      // 5.0f
    [JsonRequired, Param("Sx", 5, 15)]
    public float sx = 10f;     // 10.0f
    [JsonRequired, Param("Dy", -1, 1)]
    public float dy = 0f;      // 0.0f
    [JsonRequired, Param("Sy", 0, 2)]
    public float sy = 1f;     // 10.0f
    [JsonIgnore]
    public readonly float euler = 2.71828f;   // 2.71828f

    public override float Calc(params float[] parms)
    {
        var value = parms[0];   // X
        var de = parms[1];      // 0.0f
        var dx = parms[2];      // 0.0f
        var dy = parms[3];      // 0.0f

        return Calc(value);
    }

    public override float Calc(float v)
    {
        value = v;
        var toR = Mathf.Clamp01(((1 / (1 + Mathf.Pow(euler + de, -(value * sx) + dx))) * sy) + dy);
        return Inverted ? 1 - toR : toR;
    }

    public override int GetHashCode()
    {
        return (int)(Utils.StringToInt(GetType().ToString()) + value * 10 + de * 100 + dx * 1000 + dy * 10000 + sx * 100000 + sy * 1000000);
    }
}



