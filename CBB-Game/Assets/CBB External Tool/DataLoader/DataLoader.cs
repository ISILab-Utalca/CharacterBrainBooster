using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Utility;
using CBB.Comunication;
using System.Net.Sockets;
using Newtonsoft.Json;

public static class DataLoader
{
    private static List<Brain> brains = new List<Brain>();
    private static PairBrainData table;

    public static PairBrainData Table
    {
        get
        {
            if(table == null)
            {
                LoadTable(Path);
            }
            return table;
        }
        set
        {
            table = value;
        }
    }

    public static string Path
    {
        get
        {
#if UNITY_EDITOR
            // load considerations from the editor folder
            return Application.dataPath + "/Resources";
#else
            // load considerations from the build folder
            var dataPath = Application.dataPath;
            var path = dataPath.Replace("/" + Application.productName +"_Data", "");
            return path;
#endif
        }
    }

    static DataLoader()
    {
        Application.quitting += () => OnApplicationQuit();
    }

    /// <summary>
    /// This function is called when loading the game, 
    /// Load Data from the resources folder and save it
    /// in the static variables of the class 
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Init()
    {
        LoadBrain(Path + "/Brains");

        LoadTable(Path);

        Server.OnNewClientConnected += SendBrains;
    }

    /// <summary>
    /// this function is called when the application is closed
    /// </summary>
    private static void OnApplicationQuit()
    {
        SaveTable(Path);
    }

    private static List<PairBrainData.PairBrain> CheckPairTableConsistency()
    {
        var tor = new List<PairBrainData.PairBrain>();
        foreach (var pair in Table.pairs)
        {
            if(!brains.Any(b => b.brain_ID == pair.brain_ID))
            {
                tor.Add(pair);
            }
        }

        if(tor.Count > 0)
        {
            DebugConsistency(tor);
        }

        return tor;
    }

    private static void DebugConsistency(List<PairBrainData.PairBrain> pairs)
    {
        var msg = "Los siguientes cerebros no exiten pero un agente esta pareado a ellos:";
        foreach (var pair in pairs)
        {
            msg += "\n" + "Brain: " + pair.brain_ID + " - Agent: " + pair.agent_ID;
        }
        Debug.LogWarning(msg);
    }

    #region #PAIR-METHODS
    internal static PairBrainData.PairBrain GetPairByAgentID(string agent_ID)
    {
        var pair = Table.pairs.Find(x => x.agent_ID == agent_ID);
        return pair;
    }

    public static void AddPair(PairBrainData.PairBrain pair)
    {
        Table.Add(pair);
    }

    public static void ReplacePair(PairBrainData.PairBrain pair)
    {
        var index = Table.pairs.FindIndex(x => x.agent_ID == pair.agent_ID);
        Table.pairs[index] = pair;
    }

    public static void RemovePair(string agent_ID)
    {
        var pair = Table.pairs.Find(x => x.agent_ID == agent_ID);
        Table.pairs.Remove(pair);
    }
    #endregion

    #region #TABLE-METHODS
    public static void LoadTable(string path)
    {
        // Load brain data
        try
        {
            Table = Utility.JSONDataManager.LoadData<PairBrainData>(path, "PairsBrains", "Data");
        }
        catch (System.Exception e)
        {
            Table = new PairBrainData();
            Utility.JSONDataManager.SaveData(path, "PairsBrains", "Data", Table);
        }
    }

    public static void SaveTable(string path)
    {
        if(Table != null)
        {
            Utility.JSONDataManager.SaveData(path, "PairsBrains", "Data", Table);
        }
        else
        {
            Debug.LogWarning("No pairs to save.");
        }
    }
    #endregion

    #region BRAIN-METHODS
    /// <summary>
    /// get loaded brain by name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static Brain GetBrainByID(string name)
    {
        return brains.First(m => name.Equals(m.brain_ID));
    }

    /// <summary>
    /// Get loaded brain by index
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    public static Brain GetBrain(int i)
    {
        return brains[i];
    }
    public static List<Brain> GetAllBrains()
    {
        LoadBrain(Path + "/Brains");
        return brains;
    }
    private static void LoadBrain(string root)
    {
        System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(root);

        if(!dir.Exists)
        {
            dir.Create();
        }
        // Clear the brains list to avoid duplicates
        brains.Clear();
        var files = dir.GetFiles("*.brain");
        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].FullName.EndsWith(".brain"))
            {
                var brain = JSONDataManager.LoadData<Brain>(files[i].DirectoryName, files[i].Name);
                brains.Add(brain);
            }
        }
        Debug.Log("Loaded: " + brains.Count + " brains.");
    }

    public static void SaveBrain(string AgentID, Brain brain)
    {
        JSONDataManager.SaveData(Path + "/Brains", brain.brain_ID, "brain", brain);

        LoadBrain(Path + "/Brains");
    }
    /// <summary>
    /// When a new client connects, sends all the brains to it
    /// </summary>
    public static void SendBrains(TcpClient client)
    {
        var brains = GetAllBrains();
        var settings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto,
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            //Formatting = Formatting.Indented
        };

        var serializedBrains = JsonConvert.SerializeObject(brains, settings);
        Debug.Log(serializedBrains);
        Server.SendMessageToClient(client, serializedBrains);
    }
    #endregion
}

[System.Serializable]
public class PairBrainData
{
    [System.Serializable]
    public class PairBrain
    {
        public string agent_ID;
        public string brain_ID;
    }

    public List<PairBrain> pairs = new List<PairBrain>();

    internal void Add(PairBrain pairBrain)
    {
        if(pairs.Any(p => p.agent_ID == pairBrain.agent_ID))
        {
            Debug.LogWarning("Agent ID already exists.");
            return;
        }
        
        pairs.Add(pairBrain);
    }


    //public Brain GetBrain(string id)
    //{
    //    var pair = pairs.Find(x => x.agent_ID == id);
    //    return pairs.Find(x => x.brain_ID == pair.brain_ID);
    //}

}