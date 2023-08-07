using ArtificialIntelligence.Utility;
using CBB.Comunication;
using CBB.Lib;
using Newtonsoft.Json;
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
        public AgentData state;

        public AgentWrapper(Type type, AgentData state)
        {
            this.type = type;
            this.state = state;
        }
    }

    /// <summary>
    /// Allows an agent to send its data towards the external CBB server.
    /// </summary>
    [RequireComponent(typeof(IAgent))]
    public class AgentDataSender : MonoBehaviour
    {
        private IAgent agentComp;
        private AgentBrain agentBrain;
        
        private void Awake()
        {
            agentComp = GetComponent<IAgent>();
            agentBrain = GetComponent<AgentBrain>();
            

        }
        private void Start()
        {

            foreach (Sensor sensor in agentBrain.Sensors)
            {
                sensor.OnSensorUpdate += ReceiveSensorUpdateHandler;
            }
            agentBrain.OnDecisionTaken += ReceiveDecisionHandler;
            SendData(AgentWrapper.Type.NEW);
        }

        private void ReceiveSensorUpdateHandler()
        {
            SendData(AgentWrapper.Type.CURRENT);
        }

        private void ReceiveDecisionHandler(Option best, List<Option> otherOptions)
        {
            var agentState = agentComp.GetInternalState();

            var decisionPackage = new DecisionPackage
            {
                agentType = agentState.AgentType,
                agentName = gameObject.name,
                bestOption = CreateDecisionData(best),
                otherOptions = new List<DecisionData>()
            };
            foreach (Option option in otherOptions)
            {
                decisionPackage.otherOptions.Add(CreateDecisionData(option));
            }
            SendData(decisionPackage);
            // We also need to send the agent state when OnDecisionTaken is fired
            SendData();
        }

        private bool ClientIsConnected()
        {
            if (!Client.IsConnected)
            {
                Debug.Log("Cannot send data since you are not connected to server.");
                return false;
            }
            return true;
        }
        private void SendData(DecisionPackage decisionPackage)
        {
            if (!ClientIsConnected()) return;

            try
            {
                var data = JSONDataManager.SerializeData(decisionPackage);
                Client.SendMessageToServer(data);
                Debug.Log($"{data}");
            }
            catch (System.Exception e)
            {
                Debug.Log($"Error: {e}");
                throw;
            }
        }

        private DecisionData CreateDecisionData(Option option)
        {
            return new DecisionData(option);
        }
        private void OnDisable()
        {
            agentBrain.OnDecisionTaken -= ReceiveDecisionHandler;
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
            if (!ClientIsConnected()) return;

            try
            {
                var state = agentComp.GetInternalState();
                var wrap = new AgentWrapper(type, state);

                List<JsonConverter> converters = new()
                {
                    new GameObjectConverter(),
                    new Vector3Converter()
                };
                var data = JSONDataManager.SerializeData(wrap,converters);
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

