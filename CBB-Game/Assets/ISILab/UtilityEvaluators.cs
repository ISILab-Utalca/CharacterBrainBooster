using CBB.Api;
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
        public override float Evaluate(params object[] param)
        {
            var parm = param.Select(p => (float)p).ToArray();

            if (param.Length != 3)
                throw new ArgumentException();

            var dif = parm[1] - parm[2];

            return (parm[0] - parm[2]) / dif * 1f;
        }
    }

    [Evaluator(name: "Multiply", "Multiplier", "Multiplicand")]
    [ParamsAllowed(typeof(float), typeof(int))]
    public class Multiply : UtilityEvaluator
    {
        public override float Evaluate(params object[] param)
        {
            var parm = param.Select(p => (float)p).ToArray();
            var v = 1f;
            foreach (var temp in parm)
            {
                v = v * temp;
            }
            return v;
        }
    }

    [Evaluator(name: "Divide", "Dividend", "Divisor")]
    [ParamsAllowed(typeof(float), typeof(int))]
    public class Divide : UtilityEvaluator
    {
        public override float Evaluate(params object[] param)
        {
            var parm = param.Select(p => (float)p).ToArray();
            var v = 1f;
            foreach (var temp in parm)
            {
                v = v / temp;
            }
            return v;
        }
    }

    [Evaluator(name: "Identity", "Value")]
    [ParamsAllowed(typeof(float), typeof(int))]
    public class Identity : UtilityEvaluator
    {
        public override float Evaluate(params object[] param)
        {
            var parm = param.Select(p => (float)p).ToArray();
            if (param.Length != 1)
                throw new ArgumentException();

            return parm[0];
        }
    }

    [Evaluator(name: "Distance (v1)", "First", "Second")]
    [ParamsAllowed(typeof(float), typeof(int))]
    public class DistanceV1 : UtilityEvaluator
    {
        public override float Evaluate(params object[] param)
        {
            var parm = param.Select(p => (float)p).ToArray();
            if (param.Length != 2)
                throw new ArgumentException();

            return (parm[0] + parm[1])/2f;
        }
    }

    [Evaluator(name: "Distance (v2)", "First", "Second")]
    [ParamsAllowed(typeof(Vector2))]
    public class DistanceV2 : UtilityEvaluator
    {
        public override float Evaluate(params object[] param)
        {
            var parm = param.Select(p => (Vector2)p).ToArray();
            if (param.Length != 2)
                throw new ArgumentException();

            return Vector2.Distance(parm[0], parm[1]);
        }
    }

    [Evaluator(name: "Distance (v3)", "First", "Second")]
    [ParamsAllowed(typeof(Vector3))]
    public class DistanceV3 : UtilityEvaluator
    {
        public override float Evaluate(params object[] param)
        {
            var parm = param.Select(p => (Vector3)p).ToArray();
            if (param.Length != 2)
                throw new ArgumentException();

            return Vector3.Distance(parm[0], parm[1]);
        }
    }

    /*
   [Evaluator("Value","B","C","D")]
   public class Valentia : UtilityEvaluator // (!) este es el unico complejo deberia estar aqui?
   {
       public override float Evaluate(params object[] param)
       {
           var parm = param.Select(p => (float)p).ToArray();

           if (param.Length != 4)
               throw new ArgumentException();

           var dif = parm[2] - parm[3]; // max - min
           var v1 = (parm[0] - parm[3]) / dif * 1f;
           var v2 = (parm[1] - parm[3]) / dif * 1f;
           return ((v1 - v2) + 0.5f) / 2f;
       }
   }
   */
}