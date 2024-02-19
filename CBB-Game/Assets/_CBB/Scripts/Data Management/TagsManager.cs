using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace CBB.DataManagement
{
    [System.Serializable]
    public class TagCollection
    {
        public string name;
        public List<string> Tags { get; set; }
        public TagCollection(string name)
        {
            this.name = name;
            Tags = new List<string>();
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

        public static void SaveTagCollection(string fileName, TagCollection tagCollection)
        {
            if (tagCollection == null) return;
            
            var filePath = Path + "/" + fileName + FILE_EXTENSION;
            
            if (!File.Exists(filePath))
            {
                // Create the file
                File.Create(filePath).Dispose();
            }

            string json = JsonConvert.SerializeObject(tagCollection, m_settings);
            File.WriteAllText(filePath, json);
        }
        public static List<TagCollection> GetAllCollections()
        {
            // Read each file in the path directory and load their contents
            List<TagCollection> collections = new();
            if (!Directory.Exists(Path))
            {
                Directory.CreateDirectory(Path);
            }
            string[] files = Directory.GetFiles(Path);
            foreach (string file in files)
            {
                if (!file.EndsWith(FILE_EXTENSION)) continue;
                
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

        public static void RemoveCollection(string name)
        {
            string filePath = GetFilePath(name);
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
