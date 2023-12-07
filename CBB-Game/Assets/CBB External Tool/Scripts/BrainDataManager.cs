using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

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

        void Start()
        {
            ExternalMonitor.OnMessageReceived += HandleBrainData;
        }
        
        
        private void HandleBrainData(string data)
        {

            try
            {
                var settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    TypeNameHandling = TypeNameHandling.Auto,
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    //Formatting = Formatting.Indented
                };
                // Deserialize back the string into a list of brains
                brains = JsonConvert.DeserializeObject<List<Brain>>(data, settings);
                if(showLogs) Debug.Log("Brain data received");
            }
            catch (System.Exception)
            {
                if (showLogs)
                {
                    //Debug.LogError("Message is not brain data");
                }
                throw;
            }
        }

        public List<Brain> GetBrains()
        {
            // TODO: Bring an updated list of brains

            return brains;
        }
    } 
}
