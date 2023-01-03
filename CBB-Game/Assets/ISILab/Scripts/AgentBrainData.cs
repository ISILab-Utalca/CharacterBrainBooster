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
    public List<ActionUtility> actions = new List<ActionUtility>();

    public AgentBrainData() { }

    public AgentBrainData(AgentData baseData, List<Consideration> considerations, List<ActionUtility> actions)
    {
        this.baseData = baseData;
        this.considerations = considerations;
        this.actions = actions;
    }
}

[System.Serializable]
public class Variable // (!) esta es data simple 
{
    [JsonRequired]
    public string name;
    [JsonRequired]
    public Type type;
    [JsonRequired]
    public Type ownerType;

    public Variable(string name, Type type,Type ownerType)
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
public class ActionInfo // (!!) mejorar nombre // (!) esta es data simple
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
public class Consideration // (!) esta es data compleja 
{
    [JsonRequired]
    public string name;
    [JsonRequired]
    public bool isPublic;
    [JsonRequired, SerializeReference]
    public UtilityEvaluator evaluator;
    [JsonRequired, SerializeReference]
    public Curve curve;
    [JsonRequired, SerializeReference]
    private List<Variable> variables; // (!!) esto deberia poder guardar variables y/o consideraciones

    public Consideration(string name,bool isPublic, List<Variable> variables, UtilityEvaluator evaluator, Curve curve)
    {
        this.name = name;
        this.isPublic = isPublic;
        this.variables = variables;
        this.evaluator = evaluator;
        this.curve = curve;
    }
}

[System.Serializable]
public class ActionUtility // (!) esta es data compleja
{
    [JsonRequired]
    public string name;
    [JsonRequired]
    public ActionInfo actionInfo;
    [JsonRequired, SerializeReference]
    public UtilityEvaluator evaluator;
    [JsonRequired, SerializeReference]
    public Curve curve;
    [JsonRequired, SerializeReference]
    private List<Variable> variables; // (!!) esto deberia poder guardar variables y/o consideraciones

    public ActionUtility(string name, ActionInfo actionInfo, UtilityEvaluator evaluator, Curve curve, List<Variable> variables)
    {
        this.name = name;
        this.actionInfo = actionInfo;
        this.evaluator = evaluator;
        this.curve = curve;
        this.variables = variables;
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