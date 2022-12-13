using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.Linq;
using CBB.Lib;
using System.Security.Cryptography;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.Reflection;
using CBB.Api;

[System.Serializable]
public class AgentData
{
    [JsonRequired]
    public Type type;

    [JsonRequired]
    public List<Consideration> considerations = new List<Consideration>();

    public AgentData() { }

    public AgentData( Type type)
    {
        this.type = type;
    }

}

[System.Serializable]
public class Variable
{
    [JsonRequired]
    public string name;
    [JsonRequired]
    public Type type;
    [JsonRequired]
    public Type ownerType;
    [JsonRequired, SerializeReference]
    public object value; // (??) esto sobra?

    public Variable(string name, Type type,Type ownerType, object value)
    {
        this.name = name;
        this.type = type;
        this.ownerType = ownerType;
        this.value = value;
    }

    public override bool Equals(object obj)
    {
        var other = (Variable)obj;
        if (other == null)
            return false;

        if (other.name.Equals(this.name) &&
            other.type.Equals(this.type) &&
            other.value.Equals(this.value))
        {
            return true;
        }

        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        var x = Utils.StringToInt(this.name);
        var xx = Utils.StringToInt(this.type.ToString()) * 10;
        var xxx = Utils.StringToInt(this.ownerType.ToString()) * 100;
        var xxxx = Utils.StringToInt(this.value.ToString()) * 1000; // (?) null
        return (x + xx + xxx + xxxx);

    }
}


[System.Serializable]
public class Consideration
{
    [JsonRequired]
    public string name;
    [JsonRequired, SerializeReference]
    public List<int> variablesHash;
    [JsonRequired, SerializeField]
    public Type evaluator;
    [JsonRequired, SerializeReference]
    public Curve curve;

    [JsonIgnore]
    private List<Variable> variables;

    public Consideration(string name, List<Variable> variables, UtilityEvaluator evaluator, Curve curve)
    {
        this.name = name;
        this.variables = variables;
        this.variablesHash = variables.Select(v => v.GetHashCode()).ToList();
        this.evaluator = evaluator.GetType();
        this.curve = curve;
    }

    public override int GetHashCode()
    {
        var x = Utils.StringToInt(this.name);
        var xx = variablesHash.Sum(v => v) * 10;
        var xxx = Utils.StringToInt(evaluator.ToString()) * 100;
        var xxxx = curve.GetHashCode() * 1000;
        return (x + xx + xxx + xxxx);
    }
}

[System.Serializable]
public class ActionInfo // (!!) mejorar nombre
{
    [JsonRequired]
    public string name;
    [JsonRequired]
    public Type type;
    [JsonRequired]
    public Action action; // (??) sobra?

    public ActionInfo(string name, Type owner, Action action)
    {
        this.name = name;
        this.type = owner;
        this.action = action;
    }

    public override int GetHashCode()
    {
        var x = Utils.StringToInt(this.name);
        var xx = Utils.StringToInt(this.type.ToString()) * 10;
        var xxx = Utils.StringToInt(this.action.ToString()) * 100; // (?) null
        return (x + xx + xxx);
    }
}

[System.Serializable]
public class AgentBaseData
{
    public int? intenceRef; // (?) sobra ?
    public List<Variable> inputs = new List<Variable>();
    public List<ActionInfo> actions = new List<ActionInfo>();

    public AgentBaseData(List<Variable> inputs, List<ActionInfo> actions, int? intenceRef = null)
    {
        this.intenceRef = intenceRef;
        this.inputs = inputs;
        this.actions = actions;
    }
}

public static class Utils
{
    public static int StringToInt(string value)
    {
        var bytes = Encoding.ASCII.GetBytes(value);
        var toR = 0;
        for (int i = 0; i < bytes.Length && i < 16; i++)
        {
            toR += bytes[i] * (int)Math.Pow(10,i);
        }
        return toR;
    }
}