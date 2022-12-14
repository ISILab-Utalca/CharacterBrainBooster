using CBB.Api;
using Newtonsoft.Json;
using System;
using System.Collections;
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

    [Evaluator(name: "Normalize", "Value", "Min", "Max")]
    [ParamsAllowed(typeof(float),typeof(int))]
    public class Normalize : UtilityEvaluator
    {
        [JsonRequired, Param("Value")]
        public float value = 0f;
        [JsonRequired, Param("Min")]
        public float min = 0f;
        [JsonRequired, Param("Max")]
        public float max = 0f;

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

    [Evaluator(name: "Multiply", "Multiplier", "Multiplicand")]
    [ParamsAllowed(typeof(float), typeof(int))]
    public class Multiply : UtilityEvaluator
    {
        [JsonRequired, Param("Multiplier")]
        public float multiplier = 0f;
        [JsonRequired, Param("Multiplicand")]
        public float multiplicand = 1f;

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

    [Evaluator(name: "Divide", "Dividend", "Divisor")]
    [ParamsAllowed(typeof(float), typeof(int))]
    public class Divide : UtilityEvaluator
    {
        [JsonRequired, Param("Dividend")]
        public float dividend = 0f;
        [JsonRequired, Param("Divisor")]
        public float divisor = 1f;

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

    [Evaluator(name: "Identity", "Value")]
    [ParamsAllowed(typeof(float), typeof(int))]
    public class Identity : UtilityEvaluator
    {
        [JsonRequired, Param("Value")]
        public float value = 0f;

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

    [Evaluator(name: "Distance (v1)", "First", "Second")]
    [ParamsAllowed(typeof(float), typeof(int))]
    public class DistanceV1 : UtilityEvaluator
    {
        [JsonRequired, Param("First")]
        public float first = 0f;
        [JsonRequired, Param("Second")]
        public float second = 0f;

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
    [ParamsAllowed(typeof(Vector2))]
    public class DistanceV2 : UtilityEvaluator
    {
        [JsonRequired, Param("First")]
        public Vector2 first = Vector2.zero;
        [JsonRequired, Param("Second")]
        public Vector2 second = Vector2.zero;

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
    [ParamsAllowed(typeof(Vector3))]
    public class DistanceV3 : UtilityEvaluator
    {
        [JsonRequired, Param("First")]
        public Vector3 first = Vector3.zero;
        [JsonRequired, Param("Second")]
        public Vector3 second = Vector3.zero;

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