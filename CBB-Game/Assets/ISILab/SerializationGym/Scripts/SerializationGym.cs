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
        public SimpleData sd;
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
        [ContextMenu("Load from Json")]
        private void LoadFromJson()
        {
            sd = JSONDataManager.LoadData<SimpleData>(pathToJson, jsonFileName + $"{testNum - 1}.json");
            Debug.Log(sd);
        }
        [ContextMenu("Object test")]
        public void ObjectTest(object o)
        {
            Debug.Log("Argument type: " +  o.GetType());
        }
        private void Start()
        {
            var o = new SimpleData();
            ObjectTest(o);
        }
        private AgentBrainData CreateNewAgent()
        {

            return new AgentBrainData();
        }
    }
    /// <summary>
    /// A simple Plain 
    /// </summary>
    [System.Serializable]
    public class SimpleData
    {
        public string name;
        public int integer;
        public SimpleData() { }
        public SimpleData(string n, int i)
        {
            this.name = n;
            this.integer = i;
        }
    }
}