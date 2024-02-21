using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace CBB.DataManagement
{
    [System.Serializable]
    public class TagCollection
    {
        public string name;
        public List<string> Groups { get; set; } = new List<string>();
        public TagCollection(string name)
        {
            this.name = name;
            Groups = new List<string>();
        }
    }
    public class TagsManager
    {
        private const string FILE_EXTENSION = ".tags";
        private static JsonSerializerSettings m_settings = new()
        {
            TypeNameHandling = TypeNameHandling.Auto,
            Formatting = Formatting.Indented
        };
        public static string Path
        {
            get
            {
                return Application.dataPath + "/_CBB/Configuration/Tags";
            }
        }

        public static void Save(string fileName, TagCollection tagCollection)
        {
            if (tagCollection == null) return;
            
            var filePath = GetFilePath(fileName);
            
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Dispose();
            }

            string json = JsonConvert.SerializeObject(tagCollection, m_settings);
            File.WriteAllText(filePath, json);
        }
        public static List<TagCollection> GetAllCollections()
        {
            List<TagCollection> collections = new();
            if (!Directory.Exists(Path))
            {
                Directory.CreateDirectory(Path);
            }
            string[] files = Directory.GetFiles(Path,"*" + FILE_EXTENSION);
            foreach (string file in files)
            {
                TagCollection collection = ReadTagCollection(file);
                if (collection != null)
                {
                    collections.Add(collection);
                }
            }
            return collections;
        }
        public static TagCollection ReadTagCollection(string filePath)
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<TagCollection>(json, m_settings);

            }
            return null;
        }

        public static void RemoveCollection(TagCollection collection)
        {
            string filePath = GetFilePath(collection.name);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                File.Delete(filePath + ".meta");
            }
        }

        private static string GetFilePath(string name)
        {
            return Path + "/" + name + FILE_EXTENSION;
        }
    }
}
