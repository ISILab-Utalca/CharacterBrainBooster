using CBB.Api;
using CBB.Lib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

[Serializable]
public static class GameData
{
    #region PROPERTIES
    public static Dictionary<int, AgentData> AgentStats { get; set; } = new();
    public static ObservableCollection<AgentData> AllAgents { get; set; } = new();
    public static Dictionary<int, ObservableCollection<DecisionPackage>> Histories { get; set; } = new();
    #endregion
    #region EVENTS
    public static Action<AgentData> OnAddAgent { get; set; }
    public static Action<AgentData> OnAgentSetAsDestroyed { get; set; }
    public static Action<DecisionPackage> OnAddDecision { get; set; }
    public static Action<ObservableCollection<string>> OnAddBrains { get; set; }
    #endregion

    #region METHODS
    public static ObservableCollection<DecisionPackage> GetHistory(int agentID)
    {
        try
        {
            return Histories[agentID];
        }
        catch
        {
            Histories.Add(agentID, new ObservableCollection<DecisionPackage>());
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
            if (!AllAgents.Contains(agent)) AllAgents.Add(agent);
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
        if (Histories.ContainsKey(decisionPackage.agentID))
        {
            Histories[decisionPackage.agentID].Add(decisionPackage);
        }
        else
        {
            Histories.Add(decisionPackage.agentID, new ObservableCollection<DecisionPackage>() { decisionPackage });
        }
        OnAddDecision?.Invoke(decisionPackage);
    }

    internal static void ClearData()
    {
        Histories.Clear();
        AgentStats.Clear();
    }

    #endregion
}
