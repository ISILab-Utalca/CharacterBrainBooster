using CBB.Api;
using CBB.Comunication;
using CBB.Lib;
using Newtonsoft.Json;
using System;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public Action<InternalMessage> OnInternalMessageReceived { get; set; }

    // Set initial settings for detecting errors on deserialization
    readonly JsonSerializerSettings settings = new()
    {
        NullValueHandling = NullValueHandling.Ignore,
        MissingMemberHandling = MissingMemberHandling.Error
    };
    #region Events
    public static Action OnClientConnected { get; set; }
    #endregion

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

        try
        {
            var agentWrapper = JsonConvert.DeserializeObject<AgentWrapper>(msg, settings);
            Debug.Log("<color=lime>[MONITOR] YAY, WE HAVE AGENT WRAPPER</color>");
            GameData.HandleAgentWrapper(agentWrapper);
            return;
        }
        catch (Exception e)
        {
            Debug.Log("<color=red>[GAME DATA MANAGER] Error on AGENT WRAPPER deserialization: </color>" + e);
        }
        try
        {
            var decisionPack = JsonConvert.DeserializeObject<DecisionPackage>(msg, settings);
            Debug.Log("<color=lime>[MONITOR] YAY, WE HAVE DECISION PACKAGE</color>");
            GameData.HandleDecisionPackage(decisionPack);
            return;
        }
        catch (Exception e)
        {
            Debug.Log("<color=red>[GAME DATA MANAGER] Error DECISION PACKAGE deserialization: </color>" + e);
        }
    }
}
