using CBB.Api;
using CBB.DataManagement;
using CBB.Lib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

[Serializable]
public static class GameData
{
    internal class DecisionsAndSensorEvents
    {
        internal List<DecisionPackage> Decisions { get; set; } = new List<DecisionPackage>();
        internal List<SensorPackage> SensorEvents { get; set; } = new List<SensorPackage>();
    }

    #region PROPERTIES
    public static Dictionary<int, AgentData> AgentStats { get; set; } = new();
    public static ObservableCollection<(int, string)> Agent_ID_Name { get; set; } = new();
    private static Dictionary<int, DecisionsAndSensorEvents> Histories { get; set; } = new();
    public static List<BrainMap> BrainMaps { get; set; } = new();
    private static int MaxItems { get; set; } = 200;
    #endregion
    #region EVENTS
    public static Action<AgentData> OnAddAgent { get; set; }
    public static Action<AgentData> OnAgentSetAsDestroyed { get; set; }
    public static Action<AgentPackage> OnAddDecision { get; set; }
    public static Action<ObservableCollection<string>> OnAddBrains { get; set; }
    #endregion

    #region METHODS
    /// <summary>
    /// Get all the decisions taken by this agent
    /// </summary>
    /// <param name="agentId">Identifier of the agent</param>
    /// <returns>A list of <see cref="DecisionPackage"/> in chronological order</returns>
    public static List<DecisionPackage> GetAgentDecisions(int agentId)
    {
        var decisions = Histories[agentId].Decisions;
        if (decisions == null)
        {
            Debug.LogError($"Decisions list of agent id {agentId} is empty");
        }
        return decisions;
    }
    /// <summary>
    /// Get all the sensor events for this particular agent
    /// </summary>
    /// <param name="agentId">Identifier of the agent</param>
    /// <returns>A list of <see cref="SensorPackage"/> in chronological order</returns>
    public static List<SensorPackage> GetAgentSensorEvents(int agentId)
    {
        var sensorEvents = Histories[agentId].SensorEvents;
        if (sensorEvents == null)
        {
            Debug.LogError($"Sensor events list of agent id {agentId} is empty");
        }
        return sensorEvents;
    }
    /// <summary>
    /// Get all the decisions and sensor events of this agent
    /// </summary>
    /// <param name="agentID">Identifier of the agent</param>
    /// <returns>A list of <see cref="AgentPackage"/> (decisions and sensor events) sorted by timestamp</returns>
    public static List<AgentPackage> GetAgentFullHistory(int agentID)
    {
        var allDecisions = Histories[agentID].Decisions;
        var allSensorEvents = Histories[agentID].SensorEvents;
        var fullHistory = new List<AgentPackage>();
        fullHistory.AddRange(allDecisions);
        fullHistory.AddRange(allSensorEvents);
        // Sort by timestamp
        fullHistory = fullHistory.OrderBy(o => o.timestamp).ToList();
        return fullHistory;
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
    /// <summary>
    /// If the agent is registered, add the sensor event to its history. Else,
    /// register the agent and add the new sensor event
    /// </summary>
    /// <param name="package"></param>
    public static void HandleSensorEventPackage(SensorPackage package)
    {

        if (Histories.TryGetValue(package.agentID, out var data))
        {
            if (data.SensorEvents.Count >= MaxItems) data.SensorEvents.RemoveAt(0);
            data.SensorEvents.Add(package);
        }
        else
        {
            RegisterNewAgentHistory(package.agentID);
            Histories[package.agentID].SensorEvents.Add(package);
        }
    }

    public static void HandleDecisionPackage(DecisionPackage package)
    {

        if (Histories.TryGetValue(package.agentID, out var data))
        {
            if (data.Decisions.Count >= MaxItems) data.Decisions.RemoveAt(0);
            data.Decisions.Add(package);
        }
        else
        {
            RegisterNewAgentHistory(package.agentID);
            Histories[package.agentID].Decisions.Add(package);
        }

    }
    private static void RegisterNewAgentHistory(int agentID)
    {
        Histories.Add(agentID, new DecisionsAndSensorEvents());
    }


    internal static void ClearData()
    {
        Histories.Clear();
        AgentStats.Clear();
        Agent_ID_Name.Clear();
        BrainMaps.Clear();
    }

    #endregion
}
