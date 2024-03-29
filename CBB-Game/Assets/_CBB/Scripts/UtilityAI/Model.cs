using ArtificialIntelligence.Utility;
using CBB.Api;
using System.Collections.Generic;
using UnityEngine;

namespace CBB.Lib
{
    [System.Serializable]
    public class CameraWraper
    {
        [SerializeField]
        public byte[] image;

        public CameraWraper(byte[] image)
        {
            this.image = image;
        }
    }
    [System.Serializable]
    public class SensorStatus
    {
        public System.Type sensorType;
        public Dictionary<string, object> configurations = new();
        public Dictionary<string, object> memory = new();
        public SensorStatus() { }
        public SensorStatus(System.Type sensorType, Dictionary<string, object> config, Dictionary<string, object> memory)
        {
            this.sensorType = sensorType;
            this.configurations = config;
            this.memory = memory;
        }
    }
    /// <summary>
    /// Holds the data of a sensor activation event.
    /// </summary>
    public class SensorActivation
    {
        public string sensorName;
        /// <summary>
        /// Who fired this activation
        /// </summary>
        public string activator;
        public string activationTime;
        /// <summary>
        /// The ID of the owner of this sensor
        /// </summary>
        public int agentID;
        /// <summary>
        /// Just set the activation time to DateTime.Now
        /// </summary>
        public SensorActivation()
        {
            activationTime = System.DateTime.Now.ToString();
        }
        public SensorActivation(string sensorName, string activator, string activationTime, int agentID)
        {
            this.sensorName = sensorName;
            this.activator = activator;
            this.activationTime = activationTime;
            this.agentID = agentID;
        }
    }
    [System.Serializable]
    public class AgentBrainData
    {
        public System.Type ownerType;
        public string brainName;
        public AgentBrainData() { }

        public AgentBrainData(System.Type owner, string name)
        {
            ownerType = owner;
            brainName = name;
        }
    }
    /// <summary>
    /// Representation of the internal stats/variables of an agent on any
    /// given time
    /// </summary>
    [System.Serializable]
    public class AgentData
    {
        public System.Type AgentType;
        public string agentName;
        public int ID;
        public AgentBrainData BrainData;
        public List<SensorStatus> SensorsData;
        public List<AgentStateVariable> InternalVariables;
        public AgentData() { }
        public AgentData(int id, string name)
        {
            agentName = name;
            ID = id;
        }
        public AgentData(System.Type agentType, AgentBrainData brainData, List<SensorStatus> sensorsData)
        {
            this.AgentType = agentType;
            this.BrainData = brainData;
            this.SensorsData = sensorsData;
        }
    }
    /// <summary>
    /// Store the relevant information of an <see cref="Option"/> 
    /// </summary>
    [System.Serializable]

    public class DecisionData
    {
        public string actionName;
        public float actionScore;
        public string targetName;
        public float scaleFactor;
        public float priority;

        public List<ConsiderationData> evaluatedConsiderations;
        public DecisionData() { }
        public DecisionData(string actionName, float actionScore, string targetName)
        {
            this.actionName = actionName;
            this.actionScore = actionScore;
            this.targetName = targetName;
        }
        public DecisionData(Option option)
        {
            actionName = option.Action.GetType().Name;
            targetName = option.Target ? option.Target.name : "No target";
            actionScore = option.Score;
            scaleFactor = option.ScaleFactor;
            priority = option.Action.actionPriority;
            evaluatedConsiderations = new List<ConsiderationData>();
            if (option.Evaluations != null)
            {
                foreach (var evaluation in option.Evaluations)
                {
                    evaluatedConsiderations.Add(new ConsiderationData(evaluation.ConsiderationName,
                                                                                evaluation.UtilityValue,
                                                                                evaluation.InputValue,
                                                                                evaluation.InputName,
                                                                                evaluation.Curve));
                }
            }
        }
    }

    [System.Serializable]
    public class AgentPackage
    {
        public int agentID;
        public string timestamp;
    }

    [System.Serializable]
    public class DecisionPackage : AgentPackage
    {
        public DecisionData bestOption;
        public List<DecisionData> otherOptions;
        public DecisionPackage() { }
    }

    [System.Serializable]
    public class SensorPackage : AgentPackage
    {
        public string sensorType;
        public string activator;
    }

    [System.Serializable]
    public class AgentStateVariable
    {
        public System.Type variableType;
        public string variableName;
        public object value;
        public AgentStateVariable() { }
        public AgentStateVariable(System.Type variableType, string variableName, object value)
        {
            this.variableType = variableType;
            this.value = value;
            this.variableName = variableName;
        }
    }

    [System.Serializable]
    public class ConsiderationData
    {
        public string ConsiderationName;
        public float UtilityValue;
        public float InputValue;
        public string EvaluatedVariableName;
        public Curve Curve;
        public ConsiderationData() { }
        public ConsiderationData(string name, float utilValue, float inputValue, string varName, Curve curve)
        {
            ConsiderationName = name;
            UtilityValue = utilValue;
            InputValue = inputValue;
            EvaluatedVariableName = varName;
            Curve = curve;
        }
    }
    /// <summary>
    /// Plain C# Object to store the configuration set
    /// on this Consideration in the editor <see cref="ConsiderationEditor"/>
    /// </summary>
    [System.Serializable]
    public class ConsiderationConfiguration : IDataItem, INameable
    {
        public string considerationName;
        public string evaluationMethod;
        [SerializeReference]
        public Curve curve;
        public bool normalizeInput;
        public float minValue;
        public float maxValue;
        public ConsiderationConfiguration() { }
        public ConsiderationConfiguration(
            string considerationName,
            Curve curve,
            string evalMethod,
            bool normalize,
            float minValue = 0,
            float maxValue = 0)
        {
            this.considerationName = considerationName;
            this.curve = curve;
            this.evaluationMethod = evalMethod;
            this.normalizeInput = normalize;
            this.maxValue = maxValue;
            this.minValue = minValue;
        }

        public void SetName(string name) => considerationName = name;
        public void SetCurve(Curve curve) => this.curve = curve;

        public void SetNormalized(bool newValue) => normalizeInput = newValue;

        internal void SetMinValue(float newValue) => minValue = newValue;
        internal void SetMaxValue(float newValue) => maxValue = newValue;
        public string GetItemName() => considerationName;
        public object GetInstance() => this;

    }
}