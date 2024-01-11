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
        public static System.Action<List<Brain>> ReceivedBrains { get; set; }
        void Start()
        {
            ExternalMonitor.OnMessageReceived += HandleBrainData;
        }

        private void HandleBrainData(string data)
        {

            try
            {
                // Deserialize back the string into a list of brains
                brains = JsonConvert.DeserializeObject<List<Brain>>(data, settings);
                if (showLogs) Debug.Log("Brain data received");
                ReceivedBrains?.Invoke(brains);
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
    }
}
