using CBB.Api;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CBB.Lib
{
    public abstract class UtilityEvaluator
    {
        public abstract float Evaluate(params object[] param);
        public abstract float Evaluate(object param);

        public static List<UtilityEvaluator> GetEvaluators()
        {
            IEnumerable<UtilityEvaluator> exporters = typeof(UtilityEvaluator)
            .Assembly.GetTypes()
            .Where(t => t.IsSubclassOf(typeof(UtilityEvaluator)) && !t.IsAbstract)
            .Select(t => (UtilityEvaluator)Activator.CreateInstance(t));

            return exporters.ToList();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Evaluator(name: "Normalize", "Value", "Min", "Max")]
    [System.Serializable]
    [ParamsAllowed(typeof(float), typeof(int))]
    public class Normalize : UtilityEvaluator
    {
        [JsonRequired, Param("Value")]
        public float value = 0f;
        [JsonRequired, Param("Min")]
        public float min = 0f;
        [JsonRequired, Param("Max")]
        public float max = 0f;

        public Normalize() { }

        public Normalize(float value, float min, float max)
        {
            this.value = value;
            this.min = min;
            this.max = max;
        }

        public override float Evaluate(params object[] param)
        {
            var parm = param.Select(p => (float)p).ToArray();
            if (param.Length != 3)
                throw new ArgumentException();

            value = parm[0];
            min = parm[1];
            max = parm[2];

            return this.Evaluate(value);
        }

        public override float Evaluate(object param)
        {
            value = (float)param;
            var dif = min - max;
            return (value - min) / dif * 1f;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Evaluator(name: "Multiply", "Multiplier", "Multiplicand")]
    [System.Serializable]
    [ParamsAllowed(typeof(float), typeof(int))]
    public class Multiply : UtilityEvaluator
    {
        [JsonRequired, Param("Multiplier")]
        public float multiplier = 0f;
        [JsonRequired, Param("Multiplicand")]
        public float multiplicand = 1f;

        public Multiply() { }

        public Multiply(float multiplier, float multiplicand)
        {
            this.multiplier = multiplier;
            this.multiplicand = multiplicand;
        }

        public override float Evaluate(params object[] param)
        {
            var parm = param.Select(p => (float)p).ToArray();
            if (param.Length != 2)
                throw new ArgumentException();

            multiplier = parm[0];
            multiplicand = parm[1];

            return this.Evaluate(multiplier);
        }

        public override float Evaluate(object param)
        {
            multiplier = (float)param;
            return multiplicand * multiplicand;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Evaluator(name: "Divide", "Dividend", "Divisor")]
    [System.Serializable]
    [ParamsAllowed(typeof(float), typeof(int))]
    public class Divide : UtilityEvaluator
    {
        [JsonRequired, Param("Dividend")]
        public float dividend = 0f;
        [JsonRequired, Param("Divisor")]
        public float divisor = 1f;

        public Divide() { }

        public Divide(float dividend, float divisor)
        {
            this.dividend = dividend;
            this.divisor = divisor;
        }

        public override float Evaluate(params object[] param)
        {
            var parm = param.Select(p => (float)p).ToArray();
            if (param.Length != 2)
                throw new ArgumentException();

            dividend = parm[0];
            divisor = parm[1];

            return this.Evaluate(dividend);
        }

        public override float Evaluate(object param)
        {
            dividend = (float)param;
            return dividend * divisor;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Evaluator(name: "Identity", "Value")]
    [System.Serializable]
    [ParamsAllowed(typeof(float), typeof(int))]
    public class Identity : UtilityEvaluator
    {
        [JsonRequired, Param("Value")]
        public float value = 0f;

        public Identity() { }

        public Identity(float value)
        {
            this.value = value;
        }

        public override float Evaluate(params object[] param)
        {
            var parm = param.Select(p => (float)p).ToArray();
            if (param.Length != 1)
                throw new ArgumentException();

            value = parm[0];

            return Evaluate(value);
        }

        public override float Evaluate(object param)
        {
            return (float)param;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Evaluator(name: "Distance (v1)", "First", "Second")]
    [System.Serializable]
    [ParamsAllowed(typeof(float), typeof(int))]
    public class DistanceV1 : UtilityEvaluator
    {
        [JsonRequired, Param("First")]
        public float first = 0f;
        [JsonRequired, Param("Second")]
        public float second = 0f;

        public DistanceV1() { }

        public DistanceV1(float first, float second)
        {
            this.first = first;
            this.second = second;
        }

        public override float Evaluate(params object[] param)
        {
            var parm = param.Select(p => (float)p).ToArray();
            if (param.Length != 2)
                throw new ArgumentException();

            first = parm[0];
            second = parm[1];

            return Evaluate(first);
        }

        public override float Evaluate(object param)
        {
            first = (float)param;
            return Mathf.Abs(first - second);
        }
    }

    [Evaluator(name: "Distance (v2)", "First", "Second")]
    [System.Serializable]
    [ParamsAllowed(typeof(Vector2))]
    public class DistanceV2 : UtilityEvaluator
    {
        [JsonRequired, Param("First")]
        public Vector2 first = Vector2.zero;
        [JsonRequired, Param("Second")]
        public Vector2 second = Vector2.zero;

        public DistanceV2() { }

        public DistanceV2(Vector2 first, Vector2 second)
        {
            this.first = first;
            this.second = second;
        }

        public override float Evaluate(params object[] param)
        {
            var parm = param.Select(p => (Vector2)p).ToArray();
            if (param.Length != 2)
                throw new ArgumentException();

            first = parm[0];
            second = parm[1];

            return Evaluate(first);
        }

        public override float Evaluate(object param)
        {
            first = (Vector2)param;
            return Vector2.Distance(first, second);
        }
    }

    [Evaluator(name: "Distance (v3)", "First", "Second")]
    [System.Serializable]
    [ParamsAllowed(typeof(Vector3))]
    public class DistanceV3 : UtilityEvaluator
    {
        [JsonRequired, Param("First")]
        public Vector3 first = Vector3.zero;
        [JsonRequired, Param("Second")]
        public Vector3 second = Vector3.zero;

        public DistanceV3() { }

        public DistanceV3(Vector3 first, Vector3 second)
        {
            this.first = first;
            this.second = second;
        }

        public override float Evaluate(params object[] param)
        {
            var parm = param.Select(p => (Vector3)p).ToArray();
            if (param.Length != 2)
                throw new ArgumentException();

            first = parm[0];
            second = parm[1];

            return Evaluate(first);
        }

        public override float Evaluate(object param)
        {
            first = (Vector3)param;
            return Vector3.Distance(first, second);
        }
    }
}