using CBB.Lib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace CBB.Api
{
    /// <summary>
    /// Allows an agent to send its data towards the external CBB server.
    /// </summary>
    /// (Out of summary) This class also decouples the logic responsible for
    /// sending data over the network from the original Agent class.
    [RequireComponent(typeof(Agent))]
    public class AgentDataSender : MonoBehaviour
    {
        private Agent agent;
        private void Awake()
        {
            agent = GetComponent<Agent>();
        }
        public void SendData()
        {
            if(agent != null)
            {
                Client.AddToQueue(JSONDataManager.SerializeData(agent.BasicData));
                Debug.Log($"{this} Client code called");
            }
            else
            {
                Debug.Log("Agent is null!");
            }
        }
    }
    
}

