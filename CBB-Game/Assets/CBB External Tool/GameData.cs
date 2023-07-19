using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    private string id;

    // Current agent
    private List<(string, bool)> agents = new List<(string, bool)>();

    // Dictionary<Actor, List<Desicions>>
    private Dictionary<string, List<string>> histories = new Dictionary<string, List<string>>();
    
    // List<Brain>
    private List<string> brains = new List<string>();

    public List<string> GetHistory(string agent)
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

    public void UpdateHistory(string agent, string decision)
    {
        var history = GetHistory(agent);
        history.Add(decision);
    }

    public GameData(string id)
    {
        this.id = id;
    }

}
