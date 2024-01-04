using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CBB.ExternalTool
{
    /// <summary>
    /// Handle the brain data received from the server
    /// </summary>
    public class BrainDataManager : MonoBehaviour
    {
        [SerializeField]
        private bool showLogs = false;

        private List<Brain> brains = new();
        private readonly JsonSerializerSettings settings = new()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            // This is an important property in order to avoid type errors when deserializing
            TypeNameHandling = TypeNameHandling.Auto,
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            Formatting = Formatting.Indented
        };
        void Start()
        {
            ExternalMonitor.OnMessageReceived += HandleBrainData;
        }
        public IList<TreeViewItemData<IDataItem>> TreeRoots
        {
            get
            {
                int id = 0;
                // First level: brains
                var roots = new List<TreeViewItemData<IDataItem>>(brains.Count);
                foreach (var brain in brains)
                {
                    // This is an intermediate list to store the sensors and actions of the brain
                    var brainDataGroup = new List<TreeViewItemData<IDataItem>>();

                    // Second level: actions and sensors
                    var actionWrapper = new ItemWrapper("Actions");
                    var actionItems = new List<TreeViewItemData<IDataItem>>();
                    foreach (var action in brain.serializedActions)
                    {
                        // Third level: considerations attached to this action
                        var considerationItems = new List<TreeViewItemData<IDataItem>>();
                        foreach (var value in action.Values)
                        {
                            if (value is WrapperConsideration wc)
                            {
                                considerationItems.Add(new TreeViewItemData<IDataItem>(id++, wc.configuration));
                            }
                        }
                        actionItems.Add(new TreeViewItemData<IDataItem>(id++, action, considerationItems));
                    }

                    var sensorWrapper = new ItemWrapper("Sensors");
                    var sensorItems = new List<TreeViewItemData<IDataItem>>(brain.serializedSensors.Count);
                    foreach (var sensor in brain.serializedSensors)
                    {
                        sensorItems.Add(new TreeViewItemData<IDataItem>(id++, sensor));
                    }

                    // Production version, with children
                    brainDataGroup.Add(new TreeViewItemData<IDataItem>(id++, actionWrapper, actionItems));
                    brainDataGroup.Add(new TreeViewItemData<IDataItem>(id++, sensorWrapper, sensorItems));
                    roots.Add(new TreeViewItemData<IDataItem>(id++, brain, brainDataGroup));
                }
                return roots;
            }
        }
        class ItemWrapper : IDataItem
        {
            readonly string name;
            public ItemWrapper(string name)
            {
                this.name = name;
            }
            public string GetItemName()
            {
                return name;
            }
        }

        private void HandleBrainData(string data)
        {

            try
            {
                // Deserialize back the string into a list of brains
                brains = JsonConvert.DeserializeObject<List<Brain>>(data, settings);
                if (showLogs) Debug.Log("Brain data received");

            }
            catch (System.Exception)
            {
                //if (showLogs)
                //{
                //    Debug.LogError("Message is not brain data");
                //}
                //throw;
            }
        }

        // Method to save the brains on disk
        [ContextMenu("Save loaded brains")]
        public void SaveLoadedBrains()
        {
            if (brains == null || brains.Count == 0) return;
            string path = "C:\\Users\\diego\\Escritorio\\Docs\\CBB\\Loaded Brains\\";
            foreach (var brain in brains)
            {
                string json = JsonConvert.SerializeObject(brain, settings);
                System.IO.File.WriteAllText(path + brain.brain_ID + ".json", json);
            }
        }

        public List<Brain> GetBrains()
        {
            // TODO: Bring an updated list of brains

            return brains;
        }
    }
}
