using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UtilitySystem
{

}

public abstract class UtilityEvaluator
{
    public abstract float Evaluate(params float[] param);
}

public class A : UtilityEvaluator
{
    public override float Evaluate(params float[] param)
    {
        return 1;
    }
}

public class Normalize : UtilityEvaluator
{
    public override float Evaluate(params float[] param)
    {
        if(param[1] > param[0])
        {
            return param[1];
        }

        return param[0] / param[1];
    }
}


public class Multiply : UtilityEvaluator
{
    public override float Evaluate(params float[] param)
    {
        var v = 1f;
        foreach (var temp in param)
        {
            v = v * temp;
        }
        return v;
    }
}

public class Identity : UtilityEvaluator
{
    public override float Evaluate(params float[] param)
    {
        if (param.Length != 1)
            throw new ArgumentException();

        return param[0];
    }
}