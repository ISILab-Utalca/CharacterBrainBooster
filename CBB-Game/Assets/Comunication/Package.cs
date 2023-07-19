using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CBB.Comunication
{
    [System.Serializable]
    public class Package
    {
        public enum Type
        {
            Connect,
            Disconnect,
            Data,
        }

        [JsonRequired]
        public Type type;

        [JsonRequired, SerializeReference]
        public object data;
    }
}
