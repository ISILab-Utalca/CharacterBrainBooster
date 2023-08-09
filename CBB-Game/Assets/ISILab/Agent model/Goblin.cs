using ArtificialIntelligence.Utility;
using CBB.Api;
using UnityEngine;
namespace CBB.Lib
{
    public class Goblin : MonoBehaviour, IAgent
    {
        [UtilityInput("Health")]
        public float Health = 200f;
        [UtilityInput("Speed")]
        public float Speed = 1.0f;
        [UtilityInput("Attack")]
        public int Attack = 10;
        [UtilityInput("Can attack")]
        public bool CanAttack { get;set; }
        public AgentData AgentData { get; set; }

        private void Awake()
        {
            InitializeInternalState();
        }
        public AgentData GetInternalState()
        {
            UpdateInternalState();
            return AgentData;
        }
        [ContextMenu("New goblin internal state")]
        public void InitializeInternalState()
        {
            AgentData = new AgentData
            {
                AgentType = typeof(Goblin),
                SensorsData = new(),
                BrainData = new(typeof(Goblin),gameObject.name),
                InternalVariables = null
            };
            // Find sensors on this agent
            var sensors = gameObject.GetComponentsOnHierarchy<Sensor>();
            Debug.Log($"Total sensors on {gameObject.name}: " + sensors.Count);
            Debug.Log($"Sensors on {gameObject.name}: " + sensors);
            foreach ( var sensor in sensors )
            {
                AgentData.SensorsData.Add(sensor.SensorData);
            }
            UpdateInternalState();
        }

        public void UpdateInternalState()
        {
            AgentData.InternalVariables = UtilitySystem.CollectAgentInternalState(this);
        }
    }
}
