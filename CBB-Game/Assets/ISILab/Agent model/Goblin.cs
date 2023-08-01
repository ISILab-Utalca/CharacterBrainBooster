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

        public AgentData AgentData;
        public AgentData GetInternalState()
        {
            UpdateInternalState();
            return AgentData;
        }

        public void InitializeInternalState()
        {
            AgentData = new AgentData
            {
                AgentType = typeof(Goblin)
            };
            UpdateInternalState();
        }

        public void UpdateInternalState()
        {
            AgentData.fields = UtilitySystem.CollectVariables(typeof(Goblin));
            var sensors = gameObject.GetComponent<AgentBrain>()._sensors;
            foreach (var sensor in sensors)
            {
                //AgentData.SensorsData.Add(sensor.GetSensorData());
            }
        }
    }
}
