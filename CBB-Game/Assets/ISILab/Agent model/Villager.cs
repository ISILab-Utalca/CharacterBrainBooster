using ArtificialIntelligence.Utility;
using CBB.Api;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace CBB.Lib
{
    public class Villager : MonoBehaviour, IAgent
    {
        // Tipical stats for an NPC
        [AgentInternalState]
        public float Health = 100;
        [AgentInternalState]
        public float Speed = 3;

        public System.Action OnDeath { get; set; }
        public AgentData AgentData { get; set; }

        private void Update()
        {
            if (Health <= 0)
            {
                KillVillager();
            }
        }
        private void KillVillager()
        {
            Destroy(gameObject);
        }
        private void Awake()
        {
            InitializeInternalState();
        }
        public void InitializeInternalState()
        {
            AgentData = new AgentData
            {
                AgentType = typeof(Villager),
                SensorsData = new(),
                BrainData = new(typeof(Villager), gameObject.name),
                internalVariables = null
            };
            // Find sensors on this agent
            var sensors = gameObject.GetComponentsOnHierarchy<Sensor>();
            Debug.Log($"Total sensors on {gameObject.name}: " + sensors.Count);
            Debug.Log($"Sensors on {gameObject.name}: " + sensors);
            foreach (var sensor in sensors)
            {
                AgentData.SensorsData.Add(sensor.SensorData);
            }
            UpdateInternalState();
        }

        public AgentData GetInternalState()
        {
            UpdateInternalState();
            return AgentData;
        }

        public void UpdateInternalState()
        {
            AgentData.internalVariables = UtilitySystem.CollectAgentInternalState(this);
        }
    }
}
