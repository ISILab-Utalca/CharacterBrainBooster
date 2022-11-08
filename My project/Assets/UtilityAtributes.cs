using System;
using System.Collections;
using System.Collections.Generic;
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

    [System.AttributeUsage(System.AttributeTargets.Field | AttributeTargets.Property)]
    public class UtilityInputAttribute : Attribute
    {
        public UtilityInputAttribute(string name)
        {

        }
    }


    [System.AttributeUsage(System.AttributeTargets.Method | AttributeTargets.Event)]
    public class UtilityActionAttribute : Attribute
    {
        public UtilityActionAttribute(params string[] names)
        {

        }
    }


}