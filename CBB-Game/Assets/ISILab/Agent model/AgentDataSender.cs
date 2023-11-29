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
        private bool showLogs = false;

        private IAgent agentComp;
        private IAgentBrain agentBrain;
        private int agentID;
        private int decisionsSent = 0;
        private int dataSent = 0;

        public System.Action<string> OnSerializedData { get; set; }
        public System.Action<string> OnSerializedDecision { get; set; }
        public System.Action<string> OnSerializedSensor { get; set; }

        private void Awake()
        {
            agentComp = GetComponent<IAgent>();
            agentID = gameObject.GetInstanceID();
            agentBrain = GetComponent<IAgentBrain>();

            agentBrain.OnDecisionTaken += SendDecision;
            agentBrain.OnSetupDone += SubscribeToSensors;

        }
        private void OnDestroy()
        {
            agentBrain.OnDecisionTaken -= SendDecision;
            agentBrain.OnSetupDone -= SubscribeToSensors;
            SendDataToAllClients(AgentWrapper.AgentStateType.DESTROYED);
        }

        private void SendSensorUpdate(ISensor sensor)
        {
            var status = sensor.GetSensorData();

            var sensorPackage = new SensorPackage
            {
                agentID = agentID,
                timestamp = System.DateTime.Now.ToString(),
                sensorType = status.sensorType.ToString(),
                extraData = status.configurations.ToString() + "\n" + status.memory.ToString(),
            };
            SendDataToAllClients(sensorPackage);

            SendDataToAllClients();
        }

        private void SendDecision(Option best, List<Option> otherOptions)
        {
            var decisionPackage = new DecisionPackage
            {
                agentID = agentID,
                timestamp = System.DateTime.Now.ToString(),
                bestOption = new DecisionData(best),
                otherOptions = new List<DecisionData>()
            };
            foreach (Option option in otherOptions)
            {
                decisionPackage.otherOptions.Add(new DecisionData(option));
            }
            SendDataToAllClients(decisionPackage);
            // We also need to send the agent state
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
        /// <summary>
        /// Subscribe to the Update event and send its information
        /// </summary>
        private void SubscribeToSensors()
        {
            foreach (ISensor sensor in agentBrain.Sensors)
            {
                sensor.OnSensorUpdate += (s) =>
                {
                    SendSensorUpdate(s);
                };
            }
        }

        private void SendDataToAllClients(AgentPackage package)
        {
            if (!Server.IsRunning)
            {
                Debug.LogWarning("[AGENT DATA SENDER] Server is not running");
                return;
            }
            var data = JSONDataManager.SerializeData(package);
            try
            {
                Server.SendMessageToAllClients(data);
                decisionsSent++;
                if (showLogs)
                {
                    Debug.Log($"[AGENT DATA SENDER {gameObject.name}] has sent {decisionsSent} packages:\n{package}");  
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
            if (!Server.IsRunning) return;
            try
            {
                var data = SerializeAgentWrapperData(type);

                Server.SendMessageToAllClients(data);
                dataSent++;
                if (showLogs)
                {
                    Debug.Log($"[AGENT DATA SENDER {gameObject.name}] sent {dataSent} Agent Wrapper: {data}");
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

