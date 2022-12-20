using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.Linq;
using CBB.Lib;
using System.Text;


[System.Serializable]
public class AgentBrainData
{
    [JsonRequired]
    public AgentData baseData;

    [JsonRequired]
    public List<Consideration> considerations = new List<Consideration>();

    [JsonRequired]
    public List<ActionInfo> actions = new List<ActionInfo>();

    public AgentBrainData() { }

    public AgentBrainData(AgentData baseData, List<Consideration> considerations, List<ActionInfo> actions)
    {
        this.baseData = baseData;
        this.considerations = considerations;
        this.actions = actions;
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

    public Variable(string name, Type type,Type ownerType, object value)
    {
        this.name = name;
        this.type = type;
        this.ownerType = ownerType;
    }

    public override string ToString()
    {
        return "<b>Name:</b>\t\t" + name +
            "\n<b>Type:</b>\t\t" + type.ToString() +
            "\n<b>Owner type:</b>\t" + ownerType.ToString();
    }

    public override bool Equals(object obj)
    {
        var other = (Variable)obj;
        if (other == null)
            return false;

        if (other.name.Equals(this.name) &&
            other.type.Equals(this.type))
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
        return (x + xx + xxx);

    }
}

[System.Serializable]
public class Consideration
{
    [JsonRequired]
    public string name;
    [JsonRequired, SerializeReference]
    public UtilityEvaluator evaluator;
    [JsonRequired, SerializeReference]
    public Curve curve;
    [JsonRequired, SerializeReference]
    private List<Variable> variables;

    public Consideration(string name, List<Variable> variables, UtilityEvaluator evaluator, Curve curve)
    {
        this.name = name;
        this.variables = variables;
        this.evaluator = evaluator;
        this.curve = curve;
    }

    public override int GetHashCode()
    {
        var x = Utils.StringToInt(this.name);
        var xx = Utils.StringToInt(evaluator.ToString()) * 10;
        var xxx = curve.GetHashCode() * 100;
        var xxxx = variables.Sum(v => Utils.StringToInt(v.ToString()));
        return (x + xx + xxx);
    }
}

[System.Serializable]
public class ActionInfo // (!!) mejorar nombre
{
    [JsonRequired]
    public string name;
    [JsonRequired]
    public Type ownerType;

    public ActionInfo(string name, Type ownerType)
    {
        this.name = name;
        this.ownerType = ownerType;
    }

    public override int GetHashCode()
    {
        var x = Utils.StringToInt(this.name);
        var xx = Utils.StringToInt(this.ownerType.ToString()) * 10;
        return (x + xx);
    }
}

[System.Serializable]
public class AgentData
{
    [JsonRequired]
    public Type agentType;

    [JsonRequired, SerializeReference]
    public List<Variable> inputs = new List<Variable>();

    [JsonRequired, SerializeReference]
    public List<ActionInfo> actions = new List<ActionInfo>();

    public AgentData() { }

    public AgentData(Type agentType,List<Variable> inputs, List<ActionInfo> actions)
    {
        this.agentType = agentType;
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