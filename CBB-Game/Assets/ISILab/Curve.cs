using CBB.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
}

[Metadata("Linear")]
public class Linear : Curve
{
    float value = 0f;
    float m = 1f;
    float dx = 0f;
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
}

[Metadata("Exponencial invertida")]
public class ExponencialInvertida : Curve
{
    float value = 0f;   // X
    float e = 2f;       // 2f
    float dx = 0f;      // 0.0f
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
}

[Metadata("Exponencial")]
public class Exponencial : Curve
{
    float value = 0f;   // X
    float e = 2f;       // 2f
    float dx = 0f;      // 0.0f
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
}

[Metadata("Escalonada")]
public class Escalonada : Curve
{
    float value = 0f;   // X
    float e = 0.5f;     // 0.5f
    float max = 1f;     // 1f
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
}

[Metadata("Sigmoide")]
public class Sigmoide : Curve
{
    float value = 0f;   // X
    float de = 0f;      // 0.0f
    float dx = 0f;      // 0.0f
    float dy = 0f;      // 0.0f
    float euler = 2.71828f;   // 2.71828f

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
}



