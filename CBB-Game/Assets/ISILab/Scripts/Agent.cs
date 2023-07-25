using ArtificialIntelligence.Utility;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace CBB.Lib
{
    [System.Serializable]
    public class Agent : MonoBehaviour
    {
        /// <summary>
        /// Data structures beign serialized, then sent to CBB
        /// </summary>
        private AgentBasicData BasicData;
        private AgentBrainData BrainData;
        private SensorData[] sensorDatas;

        public List<SensorBaseClass> AgentSensors = new();
        [ContextMenu("Invoke Start")]
        private void Start()
        {
            BasicData = Generators.New_Agent_Basic_Data();
            Debug.Log($"{this} data: name = {BasicData.agentName}; type = {BasicData.agentType}");
        }
        [ContextMenu("Serialize sensor data")]
        private void SerializeSensorData()
        {
            string sensorsData = JSONDataManager.SerializeData(AgentSensors);
            Debug.Log(sensorsData);
        }
    }
}