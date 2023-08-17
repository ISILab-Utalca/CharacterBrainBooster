using CBB.Api;
using CBB.Lib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

[System.Serializable]
public class GameData
{
    #region FIELDS
    private TcpClient client;

    // Current agent
    private Dictionary<int, bool> agents = new();

    // Dictionary<Actor, List<Desicions>>
    private Dictionary<int, List<DecisionPackage>> histories = new();

    // List<Brain>
    private List<string> brains = new List<string>();
    #endregion

    #region EVENTS
    public Action<GameData, AgentWrapper> OnAddAgent { get; set; }
    public Action<GameData, AgentWrapper> OnAgentSetAsDestroyed { get; set; }
    public Action<GameData, AgentWrapper, DecisionPackage> OnAddDecision { get; set; }
    public Action<GameData, List<string>> OnAddBrains { get; set; }
    #endregion

    #region CONSTRUCTORS
    public GameData(TcpClient client)
    {
        this.client = client;
    }
    #endregion

    #region METHODS
    public List<DecisionPackage> GetHistory(AgentWrapper agent)
    {
        try
        {
            var history = histories[agent.state.ID];
            return history;
        }
        catch
        {
            histories.Add(agent.state.ID, new List<DecisionPackage>());
            var history = histories[agent.state.ID];
            return history;
        }
    }

    public void UpdateHistory(AgentWrapper agent, DecisionPackage decision)
    {
        var history = GetHistory(agent);
        history.Add(decision);
        OnAddDecision?.Invoke(this, agent, decision);
    }

    public void AddAgent(AgentWrapper agent)
    {
        agents.Add(agent.state.ID, true);
        OnAddAgent?.Invoke(this, agent);
    }

    public void RemoveAgent(AgentWrapper agent)
    {
        agents.Remove(agent.state.ID);
    }

    public void SetAgentAsDestroyed(AgentWrapper agent)
    {
        agents[agent.state.ID] = false;
        OnAgentSetAsDestroyed?.Invoke(this, agent);
    }
    #endregion
}
