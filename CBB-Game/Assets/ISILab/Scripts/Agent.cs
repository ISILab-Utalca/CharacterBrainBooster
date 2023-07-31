using ArtificialIntelligence.Utility;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility;

namespace CBB.Lib
{
    [System.Serializable]
    public class Agent : MonoBehaviour, IAgent
    {
        private List<SensorBaseClass> sensors;
        [SerializeField]
        private AgentData agentData;

        public List<SensorBaseClass> Sensors => new List<SensorBaseClass>(sensors); // unnecessary (?)

        private void Awake()
        {
            sensors = GetComponentsInChildren(typeof(SensorBaseClass), true)
                .Cast<SensorBaseClass>()
                .ToList();

            InitializeInternalState();
        }
        
        [ContextMenu("Serialize sensor data")]
        private void SerializeSensorData()
        {
            string sensorsData = JSONDataManager.SerializeData(sensors);
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