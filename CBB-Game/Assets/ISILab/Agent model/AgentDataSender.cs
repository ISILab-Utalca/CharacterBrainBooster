using ArtificialIntelligence.Utility;
using CBB.Comunication;
using CBB.Lib;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Utility;

namespace CBB.Api
{
    [System.Serializable]
    public class AgentWrapper // or AgentPackage 
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
        [SerializeField]
        private bool showLogs = false;
        
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

            Client.OnConnectedToServer += SendAgentInitialData;
        }
        private void OnDestroy()
        {
            agentBrain.OnDecisionTaken -= ReceiveDecisionHandler;
            agentBrain.OnSetupDone -= SubscribeToSensors;
            //SendData(AgentWrapper.Type.DESTROYED);
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
        private string SerializeAgentWrapperData(AgentWrapper.Type type = AgentWrapper.Type.CURRENT)
        {
            var state = agentComp.GetInternalState();
            state.agentName = gameObject.name;
            state.ID = agentID;
            var wrap = new AgentWrapper(type, state);

            // Why Am I doing this here?
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

        private void SendAgentInitialData()
        {
            SendData(AgentWrapper.Type.NEW);
            if (showLogs) Debug.Log("Initial data sent to the Server");
        }
        private void SendData(DecisionPackage decisionPackage)
        {
            if (!NeedAServer)
            {
                string serializedDecisionPackage = JSONDataManager.SerializeData(decisionPackage);
                OnSerializedDecision?.Invoke(serializedDecisionPackage);
                return;
            }
            if (!Server.ServerIsRunning) return;
            try
            {
                var data = JSONDataManager.SerializeData(decisionPackage);
                Server.SendMessageToAllClients(data);
                decisionsSent++;
                if (showLogs) Debug.Log($"{name} has sent {decisionsSent} decisions");
            }
            catch (System.Exception e)
            {
                Debug.Log($"Error: {e}");
                throw;
            }
        }
        public void SendData(AgentWrapper.Type type = AgentWrapper.Type.CURRENT)
        {
            if (!NeedAServer)
            {
                //string agentState = SerializeAgentWrapperData();
                //OnSerializedData?.Invoke(agentState);
                //Debug.Log("Printing agent state:\n" + agentState);
                //byte[] messageBytes = Encoding.UTF8.GetBytes(agentState);
                //Debug.Log($"Message length: {messageBytes.Length}");

                return;
            }
            if (!Server.ServerIsRunning) return;
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

