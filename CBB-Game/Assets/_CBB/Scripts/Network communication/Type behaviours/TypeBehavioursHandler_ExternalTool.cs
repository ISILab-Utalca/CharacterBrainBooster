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
        private static ExternalMonitor m_dataSender;

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
        public static void SendTypeBehaviours()
        {
            var typeBehaviours = GameData.TypeBehaviours;
            string json = JsonConvert.SerializeObject(typeBehaviours, Settings.JsonSerialization);
            if(m_dataSender == null)
                m_dataSender = GameObject.Find("External Monitor").GetComponent<ExternalMonitor>();
            m_dataSender.SendData(json);
        }
    }
}
