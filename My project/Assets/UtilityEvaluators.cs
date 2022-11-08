using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CBB.Lib
{
    public abstract class UtilityEvaluator
    {
        public abstract float Evaluate(params float[] param);
    }

    public class A : UtilityEvaluator
    {
        public override float Evaluate(params float[] param)
        {
            if (param.Length != 4)
                throw new ArgumentException();

            var dif = param[3] - param[4]; // max - min
            var v1 = (param[0] - param[4]) / dif;
            var v2 = (param[1] - param[4]) / dif;
            return ((v1 - v2) + 0.5f) / 2f;
        }
    }

    public class Normalize : UtilityEvaluator
    {
        public override float Evaluate(params float[] param)
        {
            if (param.Length != 3)
                throw new ArgumentException();

            var dif = param[1] - param[2];

            return param[0] - param[2] / dif;
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
}