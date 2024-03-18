using CBB.Lib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


/// <summary>
/// Represents a field or a property from a given Type
/// </summary>
[System.Serializable]
public class Variable // (!) esta es data simple 
{
    [JsonRequired]
    public string name;
    [JsonRequired]
    public Type type;
    [JsonRequired]
    public Type ownerType;

    public Variable(string name, Type type, Type ownerType)
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

    public Consideration(string name, bool isPublic, List<Variable> variables, UtilityEvaluator evaluator, Curve curve)
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
    [JsonRequired]
    public List<Consideration> considerations = new List<Consideration>();
    public ActionUtility(string name, ActionInfo actionInfo, UtilityEvaluator evaluator, Curve curve, List<Variable> variables, List<Consideration> considerations)
    {
        this.name = name;
        this.actionInfo = actionInfo;
        this.evaluator = evaluator;
        this.curve = curve;
        this.variables = variables;
        this.considerations = considerations;
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
            toR += bytes[i] * (int)Math.Pow(10, i);
        }
        return toR;
    }
}