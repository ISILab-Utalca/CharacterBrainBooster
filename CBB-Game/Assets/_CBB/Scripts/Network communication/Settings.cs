using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CBB.Comunication
{
    public static class Settings
    {
        public static readonly JsonSerializerSettings JsonSerialization = new()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            // This is an important property in order to avoid type errors when deserializing
            TypeNameHandling = TypeNameHandling.All,
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Error,
            Formatting = Formatting.Indented
        };
    }
}