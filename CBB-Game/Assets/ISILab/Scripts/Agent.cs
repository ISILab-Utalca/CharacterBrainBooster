using ArtificialIntelligence.Utility;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace CBB.Lib
{
    [System.Serializable]
    public class Agent : MonoBehaviour, IAgent
    {
        public List<SensorBaseClass> Sensors = new();
        [SerializeField]
        private AgentData agentData;
        private void Awake()
        {
            InitializeInternalState();
        }
        
        [ContextMenu("Serialize sensor data")]
        private void SerializeSensorData()
        {
            string sensorsData = JSONDataManager.SerializeData(Sensors);
            Debug.Log(sensorsData);
        }

        public IAgentInternalState GetInternalState()
        {
            return agentData;
        }
        private void InitializeInternalState()
        {
            agentData = AgentDataGenerators.New_Agent_Data(this);
        }
    }
}