using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace CBB.Lib
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Villager : MonoBehaviour, IAgent
    {
        // Tipical stats for an NPC
        [JsonProperty]
        public float Health = 100;
        [JsonProperty]
        public float Speed = 3;

        public System.Action OnDeath { get; set; }
        // The data member this class has
        public AgentData agentData;

        public AgentData SetInternalState()
        {
            return agentData;
        }
        public void InitializeInternalState()
        {
            agentData = new AgentData();
            agentData.AgentType = typeof(Villager);
            
        }

        public AgentData GetInternalState()
        {
            throw new System.NotImplementedException();
        }

        public void UpdateInternalState()
        {
            throw new System.NotImplementedException();
        }
    }
}
