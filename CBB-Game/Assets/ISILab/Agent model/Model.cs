using System;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
/*
* This file defines the plain classes that hold the information about
* an agent.
*/

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

        public SensorData(Type sensorType, Dictionary<string, object> config)
        {
            this.sensorType = sensorType;
            this.configurations = config;
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
    public class AgentData : IAgentInternalState
    {
        public Type agentType;
        public AgentBrainData BrainData;
        public List<SensorData> sensorsData;

        public AgentData() { }

        public AgentData(Type agentType, AgentBrainData brainData, List<SensorData> sensorsData)
        {
            this.agentType = agentType;
            this.BrainData = brainData;
            this.sensorsData = sensorsData;
        }
    }
}