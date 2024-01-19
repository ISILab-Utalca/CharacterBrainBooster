using CBB.Comunication;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
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
            InternalNetworkManager.OnServerMessageDequeued += UpdateBrain;
        }

        private static void UpdateBrain(string msg)
        {
            // Try to deserialize the message into a brain object
            try
            {
                var brain = JsonConvert.DeserializeObject<Brain>(msg, settings);
                // Update the brain file
                DataLoader.SaveBrain("",brain);
            }
            catch (Exception e)
            {
                Debug.LogError("[BRAIN LOADER] Message is not Brain type: " + e);
                throw;
            }
        }
    }
}
