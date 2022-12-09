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

[Curve(name:"Linear", "Slope", "Dx", "Dy")]
[System.Serializable]
public class Linear : Curve
{
    [JsonRequired]
    float value = 0f;
    [JsonRequired]
    float m = 1f;
    [JsonRequired]
    float dx = 0f;
    [JsonRequired]
    float dy = 0f;

    public override float Calc(params float[] parms)
    {
        var value = parms[0];
        var m = parms[1];
        var dx = parms[2];
        var dy = parms[3];

        return (m * (value + dx)) + dy;
    }

    public override float Calc(float v)
    {
        value = v;
        return (m * (value + dx)) + dy;
    }

    public override int GetHashCode()
    {
        return (int)(Utils.StringToInt(GetType().ToString()) + value * 10 + m * 100 + dx * 1000 + dy * 10000);
    }
}

[Curve(name:"Inverted exponential", "e", "Dx", "Dy")]
[System.Serializable]
public class ExponencialInvertida : Curve
{
    [JsonRequired]
    float value = 0f;   // X
    [JsonRequired]
    float e = 2f;       // 2f
    [JsonRequired]
    float dx = 0f;      // 0.0f
    [JsonRequired]
    float dy = 0f;      // 0.0f

    public override float Calc(params float[] parms)
    {
        var value = parms[0];   // X
        var e = parms[1];       // 2f
        var dx = parms[2];      // 0.0f
        var dy = parms[3];      // 0.0f

        return (1f / (Mathf.Pow(value, e) + dx)) + dy;
    }

    public override float Calc(float v)
    {
        value = v;
        return (1f / (Mathf.Pow(value, e) + dx)) + dy;
    }

    public override int GetHashCode()
    {
        return (int)(Utils.StringToInt(GetType().ToString()) + value * 10 + e * 100 + dx * 1000 + dy * 10000);
    }
}

[Curve(name: "Exponential", "e", "Dx", "Dy")]
[System.Serializable]
public class Exponencial : Curve
{
    [JsonRequired]
    float value = 0f;   // X
    [JsonRequired]
    float e = 2f;       // 2f
    [JsonRequired]
    float dx = 0f;      // 0.0f
    [JsonRequired]
    float dy = 0f;      // 0.0f

    public override float Calc(params float[] parms)
    {
        var value = parms[0];   // X
        var e = parms[1];       // 2f
        var dx = parms[2];      // 0.0f
        var dy = parms[3];      // 0.0f

        return Mathf.Pow(value + dx, e) + dy;
    }

    public override float Calc(float v)
    {
        value = v;
        return Mathf.Pow(value + dx, e) + dy;
    }

    public override int GetHashCode()
    {
        return (int)(Utils.StringToInt(GetType().ToString()) + value * 10 + e * 100 + dx * 1000 + dy * 10000);
    }
}

[Curve(name:"Staggered", "e", "Max", "Min")]
[System.Serializable]
public class Escalonada : Curve
{
    [JsonRequired]
    float value = 0f;   // X
    [JsonRequired]
    float e = 0.5f;     // 0.5f
    [JsonRequired]
    float max = 1f;     // 1f
    [JsonRequired]
    float min = 0.1f;   // 0.1f

    public override float Calc(params float[] parms)
    {
        var value = parms[0];   // X
        var e = parms[1];       // 0.5f
        var max = parms[2];     // 1f
        var min = parms[3];     // 0.1f

        return value >= e ? max : min;
    }

    public override float Calc(float v)
    {
        value = v;
        return value >= e ? max : min;
    }

    public override int GetHashCode()
    {
        return (int)(Utils.StringToInt(GetType().ToString()) + value * 10 + e * 100 + max * 1000 + min * 10000);
    }
}

[Curve(name:"Sigmoide", "De", "Dx", "Dy")] // ,"Euler")]
[System.Serializable]
public class Sigmoide : Curve
{
    [JsonRequired]
    float value = 0f;   // X
    [JsonRequired]
    float de = 0f;      // 0.0f
    [JsonRequired]
    float dx = 0f;      // 0.0f
    [JsonRequired]
    float dy = 0f;      // 0.0f
    [JsonIgnore]
    readonly float euler = 2.71828f;   // 2.71828f

    public override float Calc(params float[] parms)
    {
        var value = parms[0];   // X
        var de = parms[1];      // 0.0f
        var dx = parms[2];      // 0.0f
        var dy = parms[3];      // 0.0f

        return (1 / (1 + Mathf.Pow(euler + dx, -value + de))) + dy;
    }

    public override float Calc(float v)
    {
        value = v;
        return (1 / (1 + Mathf.Pow(euler + dx, -value + de))) + dy;
    }

    public override int GetHashCode()
    {
        return (int)(Utils.StringToInt(GetType().ToString()) + value * 10 + de * 100 + dx * 1000 + dy * 10000);
    }
}



