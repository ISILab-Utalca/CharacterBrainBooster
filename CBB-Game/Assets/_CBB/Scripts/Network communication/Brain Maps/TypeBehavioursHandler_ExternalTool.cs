using CBB.DataManagement;
using CBB.ExternalTool;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace CBB.Comunication
{
    public static class TypeBehavioursHandler_ExternalTool
    {
        public static System.Action TypeBehavioursReceived { get; set;}

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init()
        {
            ExternalMonitor.OnMessageReceived += HandleTypeBehaviours;
        }

        private static void HandleTypeBehaviours(string message)
        {
            try
            {
                GameData.TypeBehaviours = JsonConvert.DeserializeObject<List<TypeBehaviour>>(message, Settings.JsonSerialization);
                Debug.Log("Type Behaviours Deserialized");
                TypeBehavioursReceived?.Invoke();
            }
            catch (System.Exception) { }
        }
    }
}
