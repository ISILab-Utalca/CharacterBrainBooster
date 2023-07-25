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
    private Dictionary<AgentBasicData, bool> agents = new Dictionary<AgentBasicData, bool>();

    // Dictionary<Actor, List<Desicions>>
    private Dictionary<AgentBasicData, List<string>> histories = new Dictionary<AgentBasicData, List<string>>();
    
    // List<Brain>
    private List<string> brains = new List<string>();
    #endregion

    #region EVENTS
    public Action<GameData, AgentBasicData> OnAddAgent;
    public Action<GameData, AgentBasicData> OnAgentSetAsDestroyed;
    public Action<GameData, AgentBasicData, string> OnAddDecision;
    public Action<GameData, List<string>> OnAddBrains;
    #endregion

    #region CONSTRUCTORS
    public GameData(TcpClient client)
    {
        this.client = client;
    }
    #endregion

    #region METHODS
    public List<string> GetHistory(AgentBasicData agent)
    {
        try
        {
            var history = histories[agent];
            return history;
        }
        catch
        {
            histories.Add(agent,new List<string>());
            var history = histories[agent];
            return history;
        }
    }

    public void UpdateHistory(AgentBasicData agent, string decision)
    {
        var history = GetHistory(agent);
        history.Add(decision);
        OnAddDecision?.Invoke(this, agent, decision);
    }

    public void AddAgent(AgentBasicData agent)
    {
        agents.Add(agent, true);
        OnAddAgent?.Invoke(this, agent);
    }

    public void RemoveAgent(AgentBasicData agent)
    {
        agents.Remove(agent);
    }

    public void SetAgentAsDestroyed(AgentBasicData agent)
    {
        agents[agent] = false;
        OnAgentSetAsDestroyed?.Invoke(this, agent);
    }
    #endregion
}
