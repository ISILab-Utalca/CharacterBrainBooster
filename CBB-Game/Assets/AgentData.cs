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
    public List<Variable> inputs = new List<Variable>();
    //{
    //    new Variable("Hp",typeof(float),5f),
    //    new Variable("MaxHp",typeof(float),10f),
    //    new Variable("Name",typeof(string),"Max Steel")
    //};
    [JsonRequired]
    public List<Consideration> considerations = new List<Consideration>();
    //{
    //    new Consideration("percentageHP",new List<Variable>(){
    //        new Variable("Hp",typeof(float),5f),
    //        new Variable("MaxHp",typeof(float),10f)
    //    },
    //        new Normalize(),
    //        new Linear())
    //};

    public AgentData() { }

    public AgentData( Type type)
    {
        this.type = type;
    }

    public static Type[] Collect()
    {
        var types = Assembly.GetExecutingAssembly()
                       .GetTypes()
                       .Where(t => t.GetCustomAttributes(typeof(UtilityAgentAttribute), false).Length > 0)
                       .ToArray();
        return types;
    }


    [System.Serializable]
    public class Variable
    {
        [JsonRequired]
        public string name;
        [JsonRequired]
        public Type type;
        [JsonRequired, SerializeReference]
        public object value;

        public Variable(string name, Type type, object value)
        {
            this.name = name;
            this.type = type;
            this.value = value;
        }

        public override bool Equals(object obj)
        {
            var other = (Variable)obj;
            if (other == null)
                return false;

            if(other.name.Equals(this.name) && 
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
            var xxx = Utils.StringToInt(this.value.ToString()) * 100;
            return (x + xx + xxx);
                
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
            var xx = variablesHash.Sum( v => v) * 10;
            var xxx = Utils.StringToInt(evaluator.ToString()) * 100;
            var xxxx = curve.GetHashCode() * 1000;
            return (x + xx + xxx + xxxx);
        }
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