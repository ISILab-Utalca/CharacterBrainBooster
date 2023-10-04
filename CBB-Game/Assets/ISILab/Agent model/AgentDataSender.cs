using ArtificialIntelligence.Utility;
using CBB.Comunication;
using CBB.Lib;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace CBB.Api
{
    [System.Serializable]
    public class AgentWrapper // or AgentPackage 
    {
        [System.Serializable]
        public enum AgentStateType
        {
            NEW,
            CURRENT,
            DESTROYED
        }

        public AgentStateType type;
        public AgentData state;

        public AgentWrapper(AgentStateType type, AgentData state)
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
        [SerializeField]
        private bool showLogs = false;
        [SerializeField]
        private Text agentIDText;

        private IAgent agentComp;
        private IAgentBrain agentBrain;
        private int agentID;
        private int decisionsSent = 0;
        private int dataSent = 0;

        public System.Action<string> OnSerializedData { get; set; }
        public System.Action<string> OnSerializedDecision { get; set; }

        private void Awake()
        {
            agentComp = GetComponent<IAgent>();
            agentID = gameObject.GetInstanceID();
            agentBrain = GetComponent<IAgentBrain>();
            agentBrain.OnDecisionTaken += ReceiveDecisionHandler;
            agentBrain.OnSetupDone += SubscribeToSensors;

            agentIDText.text = gameObject.GetInstanceID().ToString();
            Server.OnNewClientConnected += SendAgentInitialDataToClient;
        }
        private void OnDestroy()
        {
            agentBrain.OnDecisionTaken -= ReceiveDecisionHandler;
            agentBrain.OnSetupDone -= SubscribeToSensors;
            SendDataToAllClients(AgentWrapper.AgentStateType.DESTROYED);
            Server.OnNewClientConnected -= SendAgentInitialDataToClient;

        }

        private void ReceiveSensorUpdateHandler()
        {
            SendDataToAllClients(AgentWrapper.AgentStateType.CURRENT);
        }
        private void ReceiveDecisionHandler(Option best, List<Option> otherOptions)
        {
            var decisionPackage = new DecisionPackage
            {
                agentID = agentID,
                bestOption = new DecisionData(best),
                otherOptions = new List<DecisionData>()
            };
            foreach (Option option in otherOptions)
            {
                decisionPackage.otherOptions.Add(new DecisionData(option));
            }
            SendDataToAllClients(decisionPackage);
            // We also need to send the agent state when OnDecisionTaken is fired
            SendDataToAllClients();
        }
        private string SerializeAgentWrapperData(AgentWrapper.AgentStateType type = AgentWrapper.AgentStateType.CURRENT)
        {
            var state = agentComp.GetInternalState();
            state.agentName = gameObject.name;
            state.ID = agentID;
            var wrap = new AgentWrapper(type, state);
            List<JsonConverter> converters = new()
                {
                    new GameObjectConverter(),
                    new Vector3Converter()
                };
            return JSONDataManager.SerializeData(wrap, converters);
        }
        private void SubscribeToSensors()
        {
            foreach (ISensor sensor in agentBrain.Sensors)
            {
                sensor.OnSensorUpdate += ReceiveSensorUpdateHandler;
            }
        }

        private void SendAgentInitialDataToClient(TcpClient client)
        {
            var data = SerializeAgentWrapperData(AgentWrapper.AgentStateType.NEW);
            Server.SendMessageToClient(client, data);
            Debug.Log("[AGENT DATA SENDER] Initial data sent to the Server");
        }
        private void SendDataToAllClients(DecisionPackage decisionPackage)
        {
            var data = JSONDataManager.SerializeData(decisionPackage);
            if (!NeedAServer)
            {
                if (showLogs)
                {
                    Debug.Log($"{name} has sent {decisionsSent} decisions");
                    Debug.Log($"Data sent: {data}");
                }
            }
            if (!Server.IsRunning) return;
            try
            {
                Server.SendMessageToAllClients(data);
                decisionsSent++;
                if (showLogs)
                {
                    Debug.Log($"{name} has sent {decisionsSent} decisions");
                    Debug.Log($"Data sent: {data}");
                }
            }
            catch (System.Exception e)
            {
                Debug.Log($"[AGENT DATA SENDER {gameObject.name}] Error sending data: {e}");
                throw;
            }
        }
        public void SendDataToAllClients(AgentWrapper.AgentStateType type = AgentWrapper.AgentStateType.CURRENT)
        {
            if (!NeedAServer) return;
            if (!Server.IsRunning) return;
            try
            {
                var data = SerializeAgentWrapperData(type);

                Server.SendMessageToAllClients(data);
                dataSent++;
                if (showLogs)
                {
                    Debug.Log($"{name} has sent {dataSent} data packages");
                    Debug.Log($"Data sent: {data}");
                }
            }
            catch (System.Exception e)
            {
                Debug.Log($"[AGENT DATA SENDER {gameObject.name}] Error sending data: {e}");
                throw;
            }
        }
    }

}

