using CBB.Api;
using CBB.Lib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

[Serializable]
public static class GameData
{
    #region PROPERTIES
    public static Dictionary<int, AgentData> AgentStats { get; set; } = new();
    public static ObservableCollection<(int, string)> Agent_ID_Name { get; set; } = new();
    public static Dictionary<int, ObservableCollection<AgentPackage>> Histories { get; set; } = new();
    #endregion
    #region EVENTS
    public static Action<AgentData> OnAddAgent { get; set; }
    public static Action<AgentData> OnAgentSetAsDestroyed { get; set; }
    public static Action<AgentPackage> OnAddDecision { get; set; }
    public static Action<ObservableCollection<string>> OnAddBrains { get; set; }
    #endregion

    #region METHODS
    public static void Add()
    {

    }

    public static ObservableCollection<AgentPackage> GetHistory(int agentID)
    {
        try
        {
            return Histories[agentID];
        }
        catch
        {
            Histories.Add(agentID, new ObservableCollection<AgentPackage>());
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
        if (AgentStats.ContainsKey(agent.ID)) 
            return;

        AgentStats.Add(agent.ID, agent);

        if (Agent_ID_Name.Contains((agent.ID, agent.agentName))) 
            return;

        Agent_ID_Name.Add((agent.ID, agent.agentName));
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

    internal static void HandleDecisionPackage(AgentPackage package)
    {
        if (Histories.ContainsKey(package.agentID))
        {
            var h = Histories[package.agentID];
            if(h.Count <= 200)
            {
                h.Add(package);
            }
            else
            {
                h.RemoveAt(h.Count - 1);
                h.Add(package);
            }
        }
        else
        {
            Histories.Add(package.agentID, new ObservableCollection<AgentPackage>() { package });
        }
        OnAddDecision?.Invoke(package);
    }

    internal static void ClearData()
    {
        Histories.Clear();
        AgentStats.Clear();
    }

    #endregion
}
