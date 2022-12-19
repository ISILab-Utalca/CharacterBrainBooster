using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CBB.Api
{
    /// <summary>
    /// This attribute allows the CBB system to identify the agents,
    /// these will be displayed on a general blackboard.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class UtilityAgentAttribute : Attribute
    {
        private string name;

        public string Name => name;

        public UtilityAgentAttribute(string name)
        {
            this.name = name;
        }
    }

    /// <summary>
    /// This attribute allows the CBB system to identify the fields
    /// and properties of an agenof an agent, these will be displayed on a general board.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public class UtilityInputAttribute : Attribute
    {
        private string name;
        private bool general;

        public string Name => name;
        public bool IsGeneral => general;

        public UtilityInputAttribute(string name, bool general = false)
        {
            this.name = name;
            this.general = general;
        }
    }

    /// <summary>
    /// This attribute allows the CBB system to identify the methods, events
    /// and actions of an agent, these will be exposed on a general board.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Method | AttributeTargets.Event, AllowMultiple = true)]
    public class UtilityActionAttribute : Attribute
    {
        private string name;
        private string[] inputs;

        public string Name => name;
        public List<string> Inputs => inputs.ToList();

        public UtilityActionAttribute(string name ,params string[] inputs)
        {
            this.name = name;
            this.inputs = inputs;
        }
    }

    /// <summary>
    /// This attribute allows the CBB system to identify if the action
    /// requires an external agent and which ones it allows.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Method | AttributeTargets.Event, AllowMultiple = true)]
    public class ReferenceOtherAttribute : Attribute
    {
        private ReferenceType refType;
        private Type[] allowedTypes;

        public ReferenceType RefType => refType;
        public List<Type> AllowedTypes => allowedTypes.ToList();

        public ReferenceOtherAttribute(ReferenceType refType, params Type[] types)
        {
            this.refType = refType;
            this.allowedTypes = types;
        }

        public enum ReferenceType
        {
            All,
            ByDistance
        }
    }


    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false)] // (??) pede que esto tenga que estar en otra hoja para que no se mesclen los attributos con diferentes usos
    public class CurveAttribute : Attribute
    {
        private string name;
        public string[] parms;

        public string Name => name;

        public CurveAttribute(string name, params string[] parms)
        {
            this.name = name;
            this.parms = parms;
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)] // (??) pede que esto tenga que estar en otra hoja para que no se mesclen los attributos con diferentes usos
    public class ParamAttribute : Attribute
    {
        private string name;
        private float min,max;

        public string Name => name;
        public float Min => min;
        public float Max => max;
        public ParamAttribute(string name,float min = 0f, float max = 1f)
        {
            this.name = name;
            this.min = min;
            this.max = max;
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false)] // (??) pede que esto tenga que estar en otra hoja para que no se mesclen los attributos con diferentes usos
    public class EvaluatorAttribute : Attribute
    {
        private string name;

        public string[] inputsNames;
        public string Name => name;

        public EvaluatorAttribute(string name, params string[] inputsNames)
        {
            this.name = name;
            this.inputsNames = inputsNames;
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false)] // (??) pede que esto tenga que estar en otra hoja para que no se mesclen los attributos con diferentes usos
    public class ParamsAllowedAttribute : Attribute
    {
        private Type[] parms;

        public Type[] Parms => parms;

        public bool IsAllowed(Type type)
        {
            return parms.Contains(type);
        }

        public ParamsAllowedAttribute(params Type[] parms)
        {
            this.parms = parms;
        }
    }

}