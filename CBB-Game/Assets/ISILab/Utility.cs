using CBB.Api;
using CBB.Lib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class Utility
{
    public Agent self;
    public UtilityEvaluator evaluator;
    public object action; // methodInfo or eventInfo
    public Curve curve;
    public List<Tuple<string, object>> others;

    public Utility(Agent self, UtilityEvaluator evaluator,Curve curve, object action, List<Tuple<string, object>> others = null)
    {
        this.self = self;
        this.evaluator = evaluator;
        this.action = action;
        this.curve = curve;
        this.others = others;
    }

    public float GetValue()
    {
        //var v = evaluator.Evaluate(inputs.Select(i => i.Item2));
        //return curve.Calc(v);
        return 0f;
    }

    public void Invoke()
    {
        var mi = (MethodInfo)action;
        var att = mi.GetCustomAttribute<UtilityActionAttribute>();

        var inputs = new List<Tuple<string, object>>();
        foreach (var inp in att.Inputs)
        {
            var otherInput = others.First(o => o.Item1.Equals(inp));
            inputs.Add(otherInput);
        }

        var act = (MethodInfo)action;
        act.Invoke(self,inputs.Select(i => i.Item2).ToArray());
    }
}
