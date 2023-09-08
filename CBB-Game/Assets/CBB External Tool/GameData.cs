using CBB.Api;
using CBB.Lib;
using System;
using System.Collections.Generic;

[Serializable]
public static class GameData
{
    #region FIELDS
    private static Dictionary<int, AgentData> agentsStats = new();

    // Dictionary<Actor, List<Desicions>>
    private static Dictionary<int, List<DecisionPackage>> histories = new();

    // List<Brain>
    private static List<string> brains = new List<string>();
    #endregion
    #region PROPERTIES
    public static Dictionary<int, AgentData> Agents { get => agentsStats; set => agentsStats = value; }
    public static Dictionary<int, List<DecisionPackage>> Histories { get => histories; set => histories = value; }
    #endregion
    #region EVENTS
    public static Action<AgentData> OnAddAgent { get; set; }
    public static Action<AgentData> OnAgentSetAsDestroyed { get; set; }
    public static Action<DecisionPackage> OnAddDecision { get; set; }
    public static Action<List<string>> OnAddBrains { get; set; }
    public static Action<DecisionPackage> OnDecisionPackageReceived { get; set; }
    #endregion

    #region CONSTRUCTORS

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

    public static void UpdateHistory(DecisionPackage decision)
    {
        var history = GetHistory(decision.agentID);
        history.Add(decision);
        OnAddDecision?.Invoke(decision);
    }
    public static void UpdateAgentData(AgentData agent)
    {
        if (agentsStats.ContainsKey(agent.ID))
        {
            agentsStats[agent.ID] = agent;
        }
    }
    public static void AddAgent(AgentData agent)
    {
        Agents.Add(agent.ID, agent);
        OnAddAgent?.Invoke(agent);
    }

    public static void RemoveAgent(AgentData agent)
    {
        Agents.Remove(agent.ID);
    }

    public static void SetAgentAsDestroyed(AgentData agent)
    {
        Agents[agent.ID] = null;
        OnAgentSetAsDestroyed?.Invoke(agent);
    }
    public static void HandleAgentWrapper(AgentWrapper agent)
    {
        switch (agent.type)
        {
            case AgentWrapper.AgentStateType.DESTROYED:
                RemoveAgent(agent.state);
                break;
            case AgentWrapper.AgentStateType.CURRENT:
                UpdateHistory(agent.state);
                break;
            case AgentWrapper.AgentStateType.NEW:
                AddAgent(agent.state);
                break;
            default:
                break;
        }
    }

    private static void UpdateHistory(AgentData agent)
    {
        throw new NotImplementedException();
    }

    internal static void HandleDecisionPackage(DecisionPackage decisionPackage)
    {
        OnDecisionPackageReceived?.Invoke(decisionPackage);
    }
    #endregion
}
