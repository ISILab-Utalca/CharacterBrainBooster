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
    public class AgentDataSender : MonoBehaviour
    {
        private IAgent agent;
        private void Awake()
        {
            agent = GetComponent<IAgent>();
        }
        [ContextMenu("Send agent data")]
        public void SendData()
        {
            if (agent == null)
            {
                Debug.Log("Agent is null!");
            }
            if (agent.GetInternalState() == null)
            {
                Debug.Log("Agent internal state is null!");
            }
            try
            {
                var data = JSONDataManager.SerializeData(agent.GetInternalState());
                Debug.Log($"{data}");
            }
            catch (System.Exception e)
            {
                Debug.Log($"Error: {e}");
                throw;
            }
            
        }
    }

}

