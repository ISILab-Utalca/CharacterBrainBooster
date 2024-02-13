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
        static readonly JsonSerializerSettings settings = new()
        {
            MissingMemberHandling = MissingMemberHandling.Error,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            TypeNameHandling = TypeNameHandling.Auto,
            Formatting = Formatting.Indented
        };
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
            catch (Exception e)
            {
                Debug.LogError("[BRAIN LOADER] Message is not Brain type: " + e);
                throw;
            }
        }

        private static void UpdateBrain(string msg)
        {
            var brain = JsonConvert.DeserializeObject<Brain>(msg, settings);
            DataLoader.SaveBrain(brain);
            DataLoader.BrainUpdated?.Invoke(brain);
        }
    }
}
