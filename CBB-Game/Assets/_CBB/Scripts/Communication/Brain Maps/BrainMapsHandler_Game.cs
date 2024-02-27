using CBB.DataManagement;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

namespace CBB.Comunication
{
    public static class BrainMapsHandler_Game
    {
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init()
        {
            Server.OnNewClientConnected += SendBrainMaps;
        }

        private static void SendBrainMaps(TcpClient client)
        {
            List<BrainMap> allBrainMaps = BrainMapsManager.GetAllBrainMaps();
            string json = JsonConvert.SerializeObject(allBrainMaps, Settings.JsonSerialization);
            Server.SendMessageToClient(client, json);
        }
    }
}
