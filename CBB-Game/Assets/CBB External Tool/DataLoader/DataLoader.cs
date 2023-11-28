using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Linq;
using System;
using CBB.Lib;
using UnityEngine.Windows;
using Utility;
using static UnityEngine.ParticleSystem;

public static class DataLoader
{
    public static List<Brain> brains = new List<Brain>();
    private static PairBrainData table;

    /// <summary>
    /// Get loaded brain by index
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    public static Brain GetBrain(int i)
    {
        return brains[i];
    }

    /// <summary>
    /// get loaded brain by name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static Brain GetBrainByName(string name)
    {
        return brains.First(m => name.Equals(m.brain_ID));
    }

    public static Brain GetBrainByAgentID(string id)
    {
        var pair = table.pairs.Find(x => x.agent_ID == id);
        return brains.Find(x => x.brain_ID == pair.brain_ID);
    }

    /// <summary>
    /// This function is called when loading the game, Load Data.
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Init()
    {
#if UNITY_EDITOR
        // load considerations from the editor folder
        LoadBrain(Application.dataPath + "/Resources/Brains");
        LoadPairs(Application.dataPath + "/Resources");
#else
        // load considerations from the build folder
        var dataPath = Application.dataPath;
        var path = dataPath.Replace("/" + Application.productName +"_Data", "");
        LoadBrain(path + "/Brains");
        LoadPairs(path);
#endif
        Debug.Log("Loaded: " + brains.Count + " brains.");
    }

    public static void LoadPairs(string path)
    {
        // Load brain data
        try
        {
            table = Utility.JSONDataManager.LoadData<PairBrainData>(path, "PairsBrains", "Data");
        }
        catch (System.Exception e)
        {
            table = new PairBrainData();
            Utility.JSONDataManager.SaveData(path, "PairsBrains", "Data", table);
        }
    }

    private static void LoadBrain(string root)
    {
        System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(root);

        var files = dir.GetFiles("*.brain");
        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].FullName.EndsWith(".brain"))
            {
                try
                {
                    var brain = JSONDataManager.LoadData(files[i].FullName) as Brain;
                    brains.Add(brain);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                }
            }
        }
    }
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


    //public Brain GetBrain(string id)
    //{
    //    var pair = pairs.Find(x => x.agent_ID == id);
    //    return pairs.Find(x => x.brain_ID == pair.brain_ID);
    //}

}