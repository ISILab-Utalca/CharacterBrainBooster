using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CBB.Api
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class UtilityAgentAttribute : Attribute
    {
        public UtilityAgentAttribute(string name)
        {

        }
    }

    [System.AttributeUsage(System.AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public class UtilityInputAttribute : Attribute
    {
        public UtilityInputAttribute(string name)
        {

        }
    }


    [System.AttributeUsage(System.AttributeTargets.Method | AttributeTargets.Event, AllowMultiple = true)]
    public class UtilityActionAttribute : Attribute
    {
        private string[] inputs;

        public List<string> Inputs => inputs.ToList();

        public UtilityActionAttribute(string name ,params string[] inputs)
        {
            this.inputs = inputs;
        }

    }


}