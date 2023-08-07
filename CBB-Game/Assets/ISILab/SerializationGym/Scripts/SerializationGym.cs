using UnityEngine;
using Newtonsoft.Json;
using UnityEditor;
using Utility;
using CBB.Lib;
using System.Collections.Generic;

namespace CBB.Tests
{
    /// <summary>
    /// Class that serves as a playground to (de)serialize objects
    /// of diferent complexity.
    /// </summary>
    public class SerializationGym : MonoBehaviour
    {
        public string pathToJson;
        public string jsonFileName;
        public int testNum = 0;
        
        public AgentBrainData brain = new();
        private Dictionary<string, object> data = new();
        [ContextMenu("Convert class to JSON")]
        private void ConvertToJson()
        {
            string sdJson = JsonConvert.SerializeObject(brain);
            JSONDataManager.SaveData<AgentBrainData>(pathToJson, jsonFileName + $"{testNum}.json", brain);
            Debug.Log(sdJson);

            testNum++;
            System.Diagnostics.Process.Start("explorer.exe", @"C:\Users\Diego\Desktop\Desktop\CodingPlayground");
        }
#if UNITY_EDITOR
        [ContextMenu("Get Path to Folder")]
        private void UpdatePathProperty()
        {
            pathToJson = EditorUtility.OpenFolderPanel("Save Json into folder ...", "", "");
        }
#endif
        
        [ContextMenu("Serialize dictionary")]
        private void SerializeDictionary()
        {
            data = new Dictionary<string, object>
            {
                { "Key1", 3 },
                { "Key2", true }
            };
            var temp = JsonConvert.SerializeObject(data);
            Debug.Log(temp);
        }
        [ContextMenu("Serialize List of strings")]
        private void SerializeStringList()
        {
            var ls = new List<string>
            {
                "Hola1",
                "Hola2",
                "Hola3",
                "Hola4"
            };
            var temp = JsonConvert.SerializeObject(ls,Formatting.Indented);
            Debug.Log(temp);    
        }
    }
}