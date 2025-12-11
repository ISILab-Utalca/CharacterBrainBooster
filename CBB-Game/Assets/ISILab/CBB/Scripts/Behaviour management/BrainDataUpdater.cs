using CBB.Comunication;
using CBB.DataManagement;
using Newtonsoft.Json;
using System;
using UnityEngine;


namespace CBB.InternalTool
{
    /// <summary>
    /// Updates a brain file with the data received from the server
    /// </summary>
    public class BrainDataUpdater
    {
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Start()
        {
            InternalNetworkManager.OnServerMessageDequeued += ProcessMessage;
        }

        private static void ProcessMessage(string msg)
        {
            try
            {
                UpdateBrain(msg);
            }
            catch (Exception)
            {
                // Intentionally left empty
            }
        }

        private static void UpdateBrain(string msg)
        {
            var brain = JsonConvert.DeserializeObject<Brain>(msg, Settings.JsonSerialization);
            BrainDataLoader.SaveBrain(brain);
            BrainDataLoader.BrainUpdated?.Invoke(brain);
        }
    }
}
