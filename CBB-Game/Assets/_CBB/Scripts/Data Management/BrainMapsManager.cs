using CBB.Comunication;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace CBB.DataManagement
{
    [System.Serializable]
    public class BrainMap
    {
        public string agentType;
        public List<SubgroupBrain> SubgroupsBrains { get; set; } = new List<SubgroupBrain>();
        public BrainMap(string name)
        {
            this.agentType = name;
            SubgroupsBrains = new List<SubgroupBrain>();
        }
        [System.Serializable]
        public class SubgroupBrain 
        {
            public string subgroupName;
            public string brainID;
            public SubgroupBrain(string name, string brainID)
            {
                this.subgroupName = name;
                this.brainID = brainID;
            }
        }
    }
    public class BrainMapsManager
    {
        private const string FILENAME = "Brain Maps.bm";
        public static string FolderPath
        {
            get
            {
#if UNITY_EDITOR
                return Application.dataPath + "/_CBB/Configuration";
#else
                return Application.dataPath + "/Configuration/";

#endif
            }
        }
        public static void Save(List<BrainMap> brainMaps)
        {
            if (brainMaps == null) return;

            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }
            var filePath = GetFilePath();

            if (!File.Exists(filePath))
            {
                File.Create(filePath).Dispose();
            }

            string json = JsonConvert.SerializeObject(brainMaps, Settings.JsonSerialization);
            File.WriteAllText(filePath, json);
        }
        public static List<BrainMap> GetAllBrainMaps()
        {
            string filePath = GetFilePath();
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<List<BrainMap>>(json, Settings.JsonSerialization);

            }
            return null;
        }
        private static string GetFilePath()
        {
            return FolderPath + "/" + FILENAME;
        }
    }
}
