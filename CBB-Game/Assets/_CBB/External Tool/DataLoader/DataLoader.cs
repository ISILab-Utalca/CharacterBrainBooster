using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using CBB.Comunication;
using System.Net.Sockets;
using Newtonsoft.Json;
using ArtificialIntelligence.Utility;
#if UNITY_EDITOR
using UnityEditor;
#endif

public static class DataLoader
{
    private static List<Brain> brains = new();
    private static PairBrainData table;

    public static System.Action<string> BrainUpdated { get;set; }
    public static PairBrainData Table
    {
        get
        {
            if (table == null)
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
            // load data from the editor folder
            return Application.dataPath + "/Resources";
#else
            // load data from the build folder
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
        LoadBrains(Path + "/Brains");

        LoadTable(Path);

    }

    /// <summary>
    /// this function is called when the application is closed
    /// </summary>
    private static void OnApplicationQuit()
    {
        SaveTable(Path);
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
        Debug.Log($"Pair {pair.agent_ID} added.");
    }

    public static void ReplacePair(PairBrainData.PairBrain pair, bool addPairIfNotFound = false)
    {
        var index = Table.pairs.FindIndex(x => x.agent_ID == pair.agent_ID);
        if (index < 0)
        {
            Debug.LogWarning($"[DATA LOADER] Agent ID {pair.agent_ID} not found.");
            if (addPairIfNotFound) AddPair(pair);
            return;
        }
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
        catch (System.Exception)
        {
            Table = new PairBrainData();
            Utility.JSONDataManager.SaveData(path, "PairsBrains", "Data", Table);
        }
    }

    public static void SaveTable(string path)
    {
        if (Table != null)
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
    /// Gets a brain by id. This method does not load the brain from disk,
    /// instead it returns the brain from the <b>in-memory</b> list of brains.
    /// </summary>
    /// <param name="id">The brain id</param>
    /// <returns>The <b>in-memory</b> brain with the corresponding ID</returns>
    public static Brain GetBrainByID(string id)
    {
        return brains.First(m => id.Equals(m.brain_ID));
    }
    
    public static List<Brain> GetAllBrains()
    {
        LoadBrains(Path + "/Brains");
        return brains;
    }
    
    public static void LoadBrains(string root)
    {
        System.IO.DirectoryInfo dir = new(root);
        Debug.Log("Loading brains from: " + dir.FullName);
        if (!dir.Exists)
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
                var brain = Utility.JSONDataManager.LoadData<Brain>(files[i].DirectoryName, files[i].Name);
                brains.Add(brain);
                BrainUpdated?.Invoke(brain.brain_ID);
            }
        }
        Debug.Log("Loaded: " + brains.Count + " brains.");
    }
    /// <summary>
    /// Save the in-memory brain data to disk.
    /// Optionally reloads the brains from disk.
    /// </summary>
    /// <param name="AgentID"></param>
    /// <param name="brain"></param>
    /// <param name="reloadBrains">If set, automatically load brains into memory</param>
    public static void SaveBrain(string AgentID, Brain brain, bool reloadBrains = true)
    {
        Utility.JSONDataManager.SaveData(Path + "/Brains", brain.brain_ID, "brain", brain);
        Debug.Log($"Brain {brain.brain_ID} saved to: {Path}");
        if(reloadBrains) LoadBrains(Path + "/Brains");
    }
#if UNITY_EDITOR
    // Save all brains on disk
    [MenuItem("CBB/Save all brains to disk")]
    public static void SaveAllBrains()
    {
        //LoadBrain(Path + "/Brains");
        for (int i = 0; i < brains.Count; i++)
        {
            SaveBrain(brains[i].brain_ID, brains[i]);
        }
    }
#endif
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

        var considerationEvaluationMethods = ConsiderationMethods.GetAllMethodNames();
        var serializedMethods = JsonConvert.SerializeObject(considerationEvaluationMethods, settings);
        Server.SendMessageToClient(client, serializedMethods);
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

    public List<PairBrain> pairs = new();

    internal void Add(PairBrain pairBrain)
    {
        if (pairs.Any(p => p.agent_ID == pairBrain.agent_ID))
        {
            Debug.LogWarning("Agent ID already exists.");
            return;
        }

        pairs.Add(pairBrain);
    }
}