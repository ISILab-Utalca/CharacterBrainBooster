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
        [SerializeField]
        private bool NeedAServer = false;
        private IAgent agentComp;
        private IAgentBrain agentBrain;
        
        public System.Action<string> OnSerializedData { get; set; }
        public System.Action<string> OnSerializedDecision { get; set; }
        private void Awake()
        {
            agentComp = GetComponent<IAgent>();
            agentBrain = GetComponent<IAgentBrain>();
            agentBrain.OnDecisionTaken += ReceiveDecisionHandler;
            agentBrain.OnSetupDone += SubscribeToSensors;
        }

        private void SubscribeToSensors()
        {
            foreach (Sensor sensor in agentBrain.Sensors)
            {
                sensor.OnSensorUpdate += ReceiveSensorUpdateHandler;
            }
            Debug.Log($"{gameObject.name} Agent Data Sender set up done");
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
                bestOption = new DecisionData(best),
                otherOptions = new List<DecisionData>()
            };
            foreach (Option option in otherOptions)
            {
                decisionPackage.otherOptions.Add(new DecisionData(option));
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
            if (!NeedAServer)
            {
                string serializedDecisionPackage = JSONDataManager.SerializeData(decisionPackage);
                Debug.Log(serializedDecisionPackage);
                OnSerializedDecision?.Invoke(serializedDecisionPackage);
                return;
            }
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
        
        private void OnDestroy()
        {
            agentBrain.OnDecisionTaken -= ReceiveDecisionHandler;
            agentBrain.OnSetupDone -= SubscribeToSensors;
            //SendData(AgentWrapper.Type.DESTROYED);
        }

        public void SendData(AgentWrapper.Type type = AgentWrapper.Type.CURRENT)
        {
            if (!NeedAServer)
            {
                string agentState = SerializeAgentWrapperData();
                OnSerializedData?.Invoke(agentState);
                Debug.Log("Printing agent state:\n" + agentState);
                return;
            }
            if (!ClientIsConnected()) return;

            try
            {
                var data = SerializeAgentWrapperData(type);

                Client.SendMessageToServer(data);
                Debug.Log($"{data}");
            }
            catch (System.Exception e)
            {
                Debug.Log($"Error: {e}");
                throw;
            }
        }
        private string SerializeAgentWrapperData(AgentWrapper.Type type = AgentWrapper.Type.CURRENT)
        {
            var state = agentComp.GetInternalState();
            var wrap = new AgentWrapper(type, state);

            // Why Am I doing this here?
            List<JsonConverter> converters = new()
                {
                    new GameObjectConverter(),
                    new Vector3Converter()
                };
            return JSONDataManager.SerializeData(wrap, converters);
        }
    }

}

