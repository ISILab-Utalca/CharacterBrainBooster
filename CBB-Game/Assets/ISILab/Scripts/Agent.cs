using ArtificialIntelligence.Utility;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility;

namespace CBB.Lib
{
    [System.Serializable]
    public class Agent : MonoBehaviour
    {
        private List<Sensor> sensors;
        [SerializeField]
        private AgentData agentData;

        public List<Sensor> Sensors => new List<Sensor>(sensors); // unnecessary (?)

        public static List<Sensor> GetSensors(GameObject agent)
        {
            return agent.GetComponentsInChildren(typeof(Sensor), true)
                .Cast<Sensor>()
                .ToList();
        }
        
        [ContextMenu("Serialize sensor data")]
        private void SerializeSensorData()
        {
            string sensorsData = JSONDataManager.SerializeData(sensors);
            Debug.Log(sensorsData);
        }

        public virtual AgentData GetInternalState()
        {
            return agentData;
        }
    }
}