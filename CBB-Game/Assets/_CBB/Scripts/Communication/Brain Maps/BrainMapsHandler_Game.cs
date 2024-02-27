using CBB.DataManagement;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

namespace CBB.Comunication
{
    public static class BrainMapsHandler_Game
    {
        private static readonly JsonSerializerSettings m_jsonSettings = new()
        {
            TypeNameHandling = TypeNameHandling.All,
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
            Formatting = Formatting.Indented,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore,

        };
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init()
        {
            Server.OnNewClientConnected += SendBrainMaps;
        }

        private static void SendBrainMaps(TcpClient client)
        {
            List<BrainMap> allBrainMaps = BrainMapsManager.GetAllBrainMaps();
            string json = JsonConvert.SerializeObject(allBrainMaps, m_jsonSettings);
            Server.SendMessageToClient(client, json);
        }
    }
}
