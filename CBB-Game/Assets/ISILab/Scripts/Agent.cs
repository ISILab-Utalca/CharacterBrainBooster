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
        public static void FindSensorData(IAgent agent)
        {
            // Find sensors on this agent
            var agentMb = agent as MonoBehaviour;
            if(agentMb == null)
            {
                Debug.LogError($"This {agent} can't be used as MonoBehaviuor");
            }
            
            var sensors = agentMb.gameObject.GetComponentsOnHierarchy<Sensor>();
            Debug.Log($"Total sensors on {agentMb.gameObject.name}: " + sensors.Count);
            Debug.Log($"Sensors on {agentMb.gameObject.name}: " + sensors);
            foreach (var sensor in sensors )
            {
                agent.AgentData.SensorsData.Add(sensor.SensorData);
            }
}
    }
}