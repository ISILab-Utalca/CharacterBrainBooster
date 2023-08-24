using CBB.Api;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace CBB.Comunication
{
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
                    Debug.Log(item.variableName);
                    Debug.Log(item.variableType);
                    Debug.Log(item.value);
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
