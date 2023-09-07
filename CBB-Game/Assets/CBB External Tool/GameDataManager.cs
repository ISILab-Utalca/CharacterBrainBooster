using CBB.Api;
using CBB.Comunication;
using CBB.Lib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    private readonly List<Type> deserializableTypes = new(){
                typeof(SensorData),
                typeof(AgentWrapper),
                typeof(DecisionPackage),
                typeof(DecisionData)
            };

    public static Action OnClientConnected { get; set; }
    public Action<InternalMessage> OnInternalMessageReceived { get; set; }
    // Set initial settings for detecting errors on deserialization
    readonly JsonSerializerSettings settings = new()
    {
        TypeNameHandling = TypeNameHandling.All,
        MissingMemberHandling = MissingMemberHandling.Error
    };

    #region Events

    #endregion
    void Update()
    {
        //string msg = "";
        //System.Type messageType = null;
        //object deserializedMessage = null;
        //// Find which type is represented by the msg string
        //foreach (System.Type t in deserializableTypes)
        //{
        //    settings.SerializationBinder = new GeneralBinder(t);
        //    try
        //    {
        //        deserializedMessage = JsonConvert.DeserializeObject(msg, settings);
        //        Debug.Log($"Successful deserialization: {deserializedMessage.GetType()}");
        //        messageType = t;
        //        break;
        //    }
        //    catch (System.Exception e)
        //    {
        //        Debug.Log("Incorrect type: " + e);
        //    }
        //}

        //if (messageType == null)
        //{
        //    Debug.LogWarning($"{name} didn't find any valid message type");
        //    return;
        //}
        //if (messageType == typeof(AgentWrapper))
        //{
        //    var agent = deserializedMessage as AgentWrapper;
        //    if (agent.type == AgentWrapper.Type.NEW)
        //    {
        //        OnReadAgent(agent);
        //    }
        //    else if (agent.type == AgentWrapper.Type.CURRENT)
        //    {
        //        //TODO: whatever should be done here
        //    }
        //    else if (agent.type == AgentWrapper.Type.DESTROYED)
        //    {
        //        GameData.RemoveAgent(agent);
        //    }
        //}
    }

    private void OnReadAgent(AgentWrapper agent)
    {
        GameData.AddAgent(agent);
    }
    public void HandleMessage(string msg)
    {
        if (Enum.TryParse(typeof(InternalMessage), msg, out object messageType))
        {
            switch (messageType)
            {
                case InternalMessage internalMessage:
                    Debug.Log("[MONITOR] Received message is of type Internal Message");
                    // Raised since the External Monitor needs to observe this event
                    OnInternalMessageReceived?.Invoke(internalMessage);
                    return;
                default:
                    break;
            }
        }
        //More cases
        object result = null;
        Type msgType = null;
        foreach (Type deserializableType in deserializableTypes)
        {
            if (TryDeserializeIntoData(msg, deserializableType, out result))
            {
                msgType = deserializableType;
                Debug.Log("[GAME DATA MANAGER] Deserialized correctly: " + result);
                break;
            }
        }
        if (msgType == null)
        {
            Debug.Log("<color=red>[GAME DATA MANAGER] Message is not deserializable</color>");
            return;
        }
        switch (result)
        {
            case AgentWrapper agentWrapper:
                GameData.HandleAgentWrapper(agentWrapper);
                break;
            case DecisionPackage decisionPackage:
                GameData.HandleDecisionPackage(decisionPackage);
                break;
            default:
                break;
        }
    }
    public bool TryDeserializeIntoData(string message, Type t, out object result)
    {
        // Set initial settings for detecting errors on deserialization
        JsonSerializerSettings settings = new()
        {
            TypeNameHandling = TypeNameHandling.All,
            MissingMemberHandling = MissingMemberHandling.Error,
            SerializationBinder = new GeneralBinder(t)
        };
        try
        {
            result = JsonConvert.DeserializeObject(message, settings);
            Debug.Log($"Successful deserialization: {result.GetType()}");
            return true;
        }
        catch (Exception e)
        {
            Debug.Log("<color=orange>[GAME DATA MANAGER] Error on deserialization: </color>" + e);
        }
        result = null;
        return false;
    }
}
