using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
    private string id;

    // Current agent
    private List<(string, bool)> agents = new List<(string, bool)>();

    // Dictionary<Actor, List<Desicions>>
    private Dictionary<string, List<string>> histories = new Dictionary<string, List<string>>();
    
    // List<Brain>
    private List<string> brains = new List<string>();

    public GameData()
    {

    }

}
