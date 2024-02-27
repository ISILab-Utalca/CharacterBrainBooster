using CBB.DataManagement;
using CBB.ExternalTool;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

namespace CBB.Comunication
{
    public static class BrainMapsHandler_ExternalTool
    {
        private static List<BrainMap> m_brainMaps;

        public static List<BrainMap> BrainMaps { get => m_brainMaps; set => m_brainMaps = value; }
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
                m_brainMaps = JsonConvert.DeserializeObject<List<BrainMap>>(message, Settings.JsonSerialization);
                Debug.Log("Brain Maps Deserialized");
                BrainMapsReceived?.Invoke();
            }
            catch (System.Exception) { }
        }

        private static void SendBrainMaps(TcpClient client)
        {
            List<BrainMap> allBrainMaps = BrainMapsManager.GetAllBrainMaps();
            string json = JsonConvert.SerializeObject(allBrainMaps, Settings.JsonSerialization);
            Server.SendMessageToClient(client, json);
        }
    }
}
