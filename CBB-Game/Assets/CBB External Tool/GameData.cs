using CBB.Api;
using CBB.Lib;
using System;
using System.Collections.Generic;

[Serializable]
public static class GameData
{
    #region FIELDS

    // Dictionary<Actor, List<Desicions>>
    private static Dictionary<int, List<DecisionPackage>> histories = new();

    // List<Brain>
    private static List<string> brains = new List<string>();
    #endregion
    #region PROPERTIES
    public static Dictionary<int, AgentData> AgentStats { get; set; } = new Dictionary<int, AgentData>();
    public static Dictionary<int, List<DecisionPackage>> Histories { get => histories; set => histories = value; }
    #endregion
    #region EVENTS
    public static Action<AgentData> OnAddAgent { get; set; }
    public static Action<AgentData> OnAgentSetAsDestroyed { get; set; }
    public static Action<DecisionPackage> OnAddDecision { get; set; }
    public static Action<List<string>> OnAddBrains { get; set; }
    #endregion

    #region METHODS
    public static List<DecisionPackage> GetHistory(int agentID)
    {
        try
        {
            return Histories[agentID];
        }
        catch
        {
            Histories.Add(agentID, new List<DecisionPackage>());
            var history = Histories[agentID];
            return history;
        }
    }

    public static void HandleAgentWrapper(AgentWrapper agent)
    {
        switch (agent.type)
        {
            case AgentWrapper.AgentStateType.DESTROYED:
                RemoveAgentState(agent.state);
                break;
            case AgentWrapper.AgentStateType.CURRENT:
                UpdateAgentState(agent.state);
                break;
            case AgentWrapper.AgentStateType.NEW:
                AddAgentState(agent.state);
                break;
            default:
                break;
        }
    }
    private static void AddAgentState(AgentData agent)
    {
        if (!AgentStats.ContainsKey(agent.ID))
        {
            AgentStats.Add(agent.ID, agent);
        }
        OnAddAgent?.Invoke(agent);
    }
    private static void UpdateAgentState(AgentData agent)
    {
        if (AgentStats.ContainsKey(agent.ID))
        {
            AgentStats[agent.ID] = agent;
        }
        else
        {
            AddAgentState(agent);
        }
    }
    private static void RemoveAgentState(AgentData agent)
    {
        AgentStats.Remove(agent.ID);
    }
    private static void SetAgentAsDestroyed(AgentData agent)
    {
        AgentStats[agent.ID] = null;
        OnAgentSetAsDestroyed?.Invoke(agent);
    }

    internal static void HandleDecisionPackage(DecisionPackage decisionPackage)
    {
        if (histories.ContainsKey(decisionPackage.agentID))
        {
            histories[decisionPackage.agentID].Add(decisionPackage);
        }
        else
        {
            histories.Add(decisionPackage.agentID, new List<DecisionPackage>() { decisionPackage });
        }
        OnAddDecision?.Invoke(decisionPackage);
    }
    public static void UpdateAgentHistory(DecisionPackage decision)
    {
        var history = GetHistory(decision.agentID);
        history.Add(decision);
        OnAddDecision?.Invoke(decision);
    }

    #endregion
}
