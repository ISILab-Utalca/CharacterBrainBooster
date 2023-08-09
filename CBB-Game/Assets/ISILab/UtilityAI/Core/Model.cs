using ArtificialIntelligence.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CBB.Lib
{
    /// <summary>
    /// Data used to identify this Agent Instance in the CBB tool
    /// </summary>
    [Serializable]
    public class AgentBasicData
    {
        public Type agentType;
        public string agentName;
        public AgentBasicData(Type agentType, string agentName)
        {
            this.agentType = agentType;
            this.agentName = agentName;
        }
    }

    [System.Serializable]
    public class SensorData
    {
        public Type sensorType;
        public Dictionary<string, object> configurations = new();
        public Dictionary<string, object> memory = new();
        public SensorData() { }
        public SensorData(Type sensorType, Dictionary<string, object> config, Dictionary<string, object> memory)
        {
            this.sensorType = sensorType;
            this.configurations = config;
            this.memory = memory;
        }
    }

    [System.Serializable]
    public class AgentBrainData
    {
        public Type ownerType;
        public string brainName;
        public AgentBrainData() { }

        public AgentBrainData(Type owner, string name)
        {
            ownerType = owner;
            brainName = name;
        }
    }

    [System.Serializable]
    public class AgentData
    {
        public Type AgentType;
        public AgentBrainData BrainData;
        public List<SensorData> SensorsData;
        public List<AgentStateVariable> InternalVariables;
        public AgentData() { }

        public AgentData(Type agentType, AgentBrainData brainData, List<SensorData> sensorsData)
        {
            this.AgentType = agentType;
            this.BrainData = brainData;
            this.SensorsData = sensorsData;
        }
    }
    [System.Serializable]
    public class DecisionData
    {
        public string actionName;
        public float actionScore;
        public string targetName;
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
        }
    }
    [System.Serializable]
    public class DecisionPackage
    {
        public Type agentType;
        public string agentName;
        public DecisionData bestOption;
        public List<DecisionData> otherOptions;
        public DecisionPackage() { }
        public DecisionPackage(Type agentType, string agentName, DecisionData best, List<DecisionData> otherOptions)
        {
            this.agentType = agentType;
            this.agentName = agentName;
            this.bestOption = best;
            this.otherOptions = otherOptions;
        }
    }
    [System.Serializable]
    public class AgentStateVariable
    {
        public Type variableType;
        public string variableName;
        public object value;
        public AgentStateVariable() { }
        public AgentStateVariable(Type variableType, string variableName, object value)
        {
            this.variableType = variableType;
            this.value = value;
            this.variableName = variableName;
        }
    }
}