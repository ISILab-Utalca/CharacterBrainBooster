using CBB.Api;
using Newtonsoft.Json;
using System;
using UnityEngine;

namespace CBB.Comunication
{
    /// <summary>
    /// Deserialize the data received from the server 
    /// </summary>
    public class DataDeserializer : MonoBehaviour
    {
        [SerializeField]
        private AgentDataSender agentDataSender;

        private void Start()
        {
            agentDataSender = GetComponent<AgentDataSender>();
            agentDataSender.OnSerializedData += DeserializeAgentData;
        }
        
        private void DeserializeAgentData(string serializedAgentWrapper)
        {
            try
            {
                var agentWrapper = JsonConvert.DeserializeObject<AgentWrapper>(serializedAgentWrapper);
                Debug.Log("Data serializer prints wrapper to string:");
                Debug.Log(agentWrapper.type);
                Debug.Log(agentWrapper.state.BrainData.brainName);
                Debug.Log(agentWrapper.state.BrainData.ownerType);
                foreach (var item in agentWrapper.state.SensorsData)
                {
                    foreach (var kvp in item.configurations)
                    {
                        Debug.Log($"{kvp.Key}: {kvp.Value}");
                    }
                    foreach (var kvp in item.memory)
                    {
                        Debug.Log($"{kvp.Key}: {kvp.Value}");
                    }
                }
                foreach (var item in agentWrapper.state.InternalVariables)
                {
                    Debug.Log($"Var name: {item.variableName}; var type: {item.variableType}; var value: {item.value}");
                }
                Debug.Log("Done printing wrapper");

            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }
        private void OnDisable()
        {
            agentDataSender.OnSerializedData -= DeserializeAgentData;
        }
    }
}
