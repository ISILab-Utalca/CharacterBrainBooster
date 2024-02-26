using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace CBB.DataManagement
{
    [System.Serializable]
    public class BrainMap
    {
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
        public string agentType;
        public List<SubgroupBrain> SubgroupsBrains { get; set; } = new List<SubgroupBrain>();
        public BrainMap(string name)
        {
            this.agentType = name;
            SubgroupsBrains = new List<SubgroupBrain>();
        }
    }
    public class BrainMapsManager
    {
        private const string FILENAME = "Brain Maps.bm";
        private static readonly JsonSerializerSettings m_settings = new()
        {
            TypeNameHandling = TypeNameHandling.Auto,
            Formatting = Formatting.Indented
        };
        public static string Path
        {
            get
            {
                return Application.dataPath + "/_CBB/Configuration/Brain Maps";
            }
        }
        public static void Save(List<BrainMap> brainMaps)
        {
            if (brainMaps == null) return;

            if (!Directory.Exists(Path))
            {
                Directory.CreateDirectory(Path);
            }
            var filePath = GetFilePath();

            if (!File.Exists(filePath))
            {
                File.Create(filePath).Dispose();
            }

            string json = JsonConvert.SerializeObject(brainMaps, m_settings);
            File.WriteAllText(filePath, json);
        }
        public static List<BrainMap> GetAllBrainMaps()
        {
            string filePath = GetFilePath();
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<List<BrainMap>>(json, m_settings);

            }
            return null;
        }
        private static string GetFilePath()
        {
            return Path + "/" + FILENAME;
        }
    }
}
