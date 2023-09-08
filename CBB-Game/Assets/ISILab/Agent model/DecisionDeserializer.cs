using CBB.Api;
using CBB.Lib;
using Newtonsoft.Json;
using System;
using UnityEngine;

public class DecisionDeserializer : MonoBehaviour
{
    [SerializeField]
    private AgentDataSender agentDataSender;
    private void Start()
    {
        agentDataSender = GetComponent<AgentDataSender>();
        agentDataSender.OnSerializedDecision += DeserializeDecisionData;
    }

    private void DeserializeDecisionData(string serializedDecisionPackage)
    {
        try
        {
            var decisionPackage = JsonConvert.DeserializeObject<DecisionPackage>(serializedDecisionPackage);
            Debug.Log("Data serializer prints Decision package:");
            Debug.Log(decisionPackage.bestOption.actionName);
            Debug.Log(decisionPackage.bestOption.actionScore);
            Debug.Log(decisionPackage.bestOption.targetName);

            foreach (var item in decisionPackage.otherOptions)
            {
                Debug.Log($"{item.actionName}, {item.actionScore}, {item.targetName}");
            }
            Debug.Log("Done printing Decision package");

        }
        catch (Exception e)
        {
            Debug.LogException(e);
            throw;
        }
    }
    private void OnDisable()
    {
        agentDataSender.OnSerializedDecision -= DeserializeDecisionData;
    }
}
