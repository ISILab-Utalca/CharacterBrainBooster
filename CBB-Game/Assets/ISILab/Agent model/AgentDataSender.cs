using CBB.Comunication;
using CBB.Lib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace CBB.Api
{
    [System.Serializable]
    public struct AgentWrapper // or AgentPackage 
    {
        [System.Serializable]
        public enum Type
        {
            NEW,
            CURRENT,
            DESTROYED
        }

        public Type type;
        public IAgentInternalState state;

        public AgentWrapper(Type type, IAgentInternalState state)
        {
            this.type = type;
            this.state = state;
        }
    }

    /// <summary>
    /// Allows an agent to send its data towards the external CBB server.
    /// </summary>
    [RequireComponent(typeof(Agent))]
    public class AgentDataSender : MonoBehaviour
    {
        private IAgent agent;

        private void Awake()
        {
            agent = GetComponent<IAgent>();
            SendData(AgentWrapper.Type.NEW);
        }

        private void OnDestroy()
        {
            SendData(AgentWrapper.Type.DESTROYED);
        }

        [ContextMenu("Send agent data")]
        private void ContexMenuSendData()
        {
            SendData();
        }

        public void SendData(AgentWrapper.Type type = AgentWrapper.Type.CURRENT)
        {
            if (!Client.IsConnected)
            {
                Debug.Log("Cannot send data since you are not connected to server.");
                return;
            }

            try
            {
                var state = agent.GetInternalState();
                var wrap = new AgentWrapper(type, state);

                var data = JSONDataManager.SerializeData(wrap);
                Client.SendMessageToServer(data);
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

