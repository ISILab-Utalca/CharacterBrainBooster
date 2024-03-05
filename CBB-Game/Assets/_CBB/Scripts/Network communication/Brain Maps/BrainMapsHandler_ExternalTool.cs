using CBB.DataManagement;
using CBB.ExternalTool;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace CBB.Comunication
{
    public static class BrainMapsHandler_ExternalTool
    {
        public static System.Action BrainMapsReceived { get; set;}

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init()
        {
            ExternalMonitor.OnMessageReceived += HandleBrainMaps;
        }

        private static void HandleBrainMaps(string message)
        {
            try
            {
                GameData.BrainMaps = JsonConvert.DeserializeObject<List<BrainMap>>(message, Settings.JsonSerialization);
                Debug.Log("Brain Maps Deserialized");
                BrainMapsReceived?.Invoke();
            }
            catch (System.Exception) { }
        }
    }
}
