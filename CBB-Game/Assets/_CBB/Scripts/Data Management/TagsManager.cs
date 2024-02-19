using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace CBB.DataManagement
{
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
            
            var filePath = Path + fileName + FILE_EXTENSION;
            
            if (!File.Exists(filePath))
            {
                File.Create(filePath);
            }

            string json = JsonConvert.SerializeObject(tagCollection, m_settings);
            File.WriteAllText(filePath, json);
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
                TagCollection collection = ReadTagCollection(file);
                if (collection != null)
                {
                    collections.Add(collection);
                }
            }
            return collections;
        }
    }
}
