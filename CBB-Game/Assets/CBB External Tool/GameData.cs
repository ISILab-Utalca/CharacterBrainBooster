using CBB.Api;
using CBB.Lib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public static class GameData
{
    #region FIELDS
    private static TcpClient client;

    // Current agent
    private static Dictionary<int, bool> agents = new();

    // Dictionary<Actor, List<Desicions>>
    private static Dictionary<int, List<DecisionPackage>> histories = new();

    // List<Brain>
    private static List<string> brains = new List<string>();
    #endregion
    #region PROPERTIES
    public static TcpClient Client { get => client; set => client = value; }
    public static Dictionary<int, bool> Agents { get => agents; set => agents = value; }
    public static Dictionary<int, List<DecisionPackage>> Histories { get => histories; set => histories = value; }
    #endregion
    #region EVENTS
    public static Action<AgentWrapper> OnAddAgent { get; set; }
    public static Action<AgentWrapper> OnAgentSetAsDestroyed { get; set; }
    public static Action<AgentWrapper, DecisionPackage> OnAddDecision { get; set; }
    public static Action<List<string>> OnAddBrains { get; set; }
    public static Action<DecisionPackage> OnDecisionPackageReceived { get; set; }
    #endregion

    #region CONSTRUCTORS

    #endregion

    #region METHODS
    public static List<DecisionPackage> GetHistory(AgentWrapper agent)
    {
        try
        {
            var history = Histories[agent.state.ID];
            return history;
        }
        catch
        {
            Histories.Add(agent.state.ID, new List<DecisionPackage>());
            var history = Histories[agent.state.ID];
            return history;
        }
    }

    public static void UpdateHistory(AgentWrapper agent, DecisionPackage decision)
    {
        var history = GetHistory(agent);
        history.Add(decision);
        OnAddDecision?.Invoke(agent, decision);
    }

    public static void AddAgent(AgentWrapper agent)
    {
        Agents.Add(agent.state.ID, true);
        OnAddAgent?.Invoke(agent);
    }

    public static void RemoveAgent(AgentWrapper agent)
    {
        Agents.Remove(agent.state.ID);
    }

    public static void SetAgentAsDestroyed(AgentWrapper agent)
    {
        Agents[agent.state.ID] = false;
        OnAgentSetAsDestroyed?.Invoke(agent);
    }
    public static void HandleAgentWrapper(AgentWrapper agent)
    {
        switch (agent.type)
        {
            case AgentWrapper.Type.DESTROYED:
                RemoveAgent(agent);
                break;
            case AgentWrapper.Type.CURRENT:
                //GameData.UpdateHistory(agent);
                break;
            default:
                break;
        }
    }

    internal static void HandleDecisionPackage(DecisionPackage decisionPackage)
    {
        OnDecisionPackageReceived?.Invoke(decisionPackage);
    }
    #endregion
}
