using Generic;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CBB.ExternalTool
{
    /// <summary>
    /// Handles the data received from the server.
    /// </summary>
    public class GameDataHandler : MonoBehaviour
    {
        [SerializeField]
        private bool showLogs = false;

        private readonly JsonSerializerSettings settings = new()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            // This is an important property in order to avoid type errors when deserializing
            TypeNameHandling = TypeNameHandling.All,
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore,
        };
        
        public static System.Action<List<Brain>> ReceivedBrains { get; set; }
        public static System.Action<List<DataGeneric>> ReceivedActions { get; set; }
        public static System.Action<List<DataGeneric>> ReceivedSensors { get; set; }
        public static System.Action<List<string>> ReceivedEvaluationMethods { get; internal set; }

        void Start()
        {
            ExternalMonitor.OnMessageReceived += HandleData;
        }

        private void HandleData(string data)
        {
            if(HandleBrains(data)) return;
            if(HandleDataGeneric(data)) return;
            if(HandleEvaluationMethods(data)) return;
        }

        bool HandleBrains(string data)
        {
            try
            {
                // Deserialize back the string into a list of brains
                var brains = JsonConvert.DeserializeObject<List<Brain>>(data, settings);
                if (showLogs) Debug.Log("Brain data received");
                ReceivedBrains?.Invoke(brains);
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }
        bool HandleDataGeneric(string data)
        {
            try
            {
                var dg = JsonConvert.DeserializeObject<List<DataGeneric>>(data, settings);
                if (showLogs) Debug.Log("Data generic received");
                // Test if DataGeneric is action or sensor
                switch (dg[0].GetDataType())
                {
                    case DataGeneric.DataType.Action:
                        ReceivedActions?.Invoke(dg);
                        break;
                    case DataGeneric.DataType.Curve:
                        break;
                    case DataGeneric.DataType.Sensor:
                        ReceivedSensors?.Invoke(dg);
                        break;
                    case DataGeneric.DataType.Default:
                        break;
                    default:
                        break;
                }
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }
        bool HandleEvaluationMethods(string data)
        {
            try
            {
                // Deserialize back the string into a list of brains
                var methods = JsonConvert.DeserializeObject<List<string>>(data, settings);
                if (showLogs) Debug.Log("Evaluation methods received");
                ReceivedEvaluationMethods?.Invoke(methods);
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }
    }
    
}
