using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;
/// <summary>
/// Helper class inherited from Monobehaviour. It's serialized using the
/// Attributes approach
/// </summary>
namespace CBB.Lib
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class TransformerWithAttributes : MonoBehaviour
    {
        // Fields to test serialization
        [JsonProperty]
        public int publicInt = 1;
        [JsonProperty]
        private float privFloat = 1.0f;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public float PrivFloatProperty { get { return privFloat; } }

        [ContextMenu("save data to json")]
        public void BrainToJson()
        {
            string s = JSONDataManager.SerializeData(this);
            Debug.Log(s);
        }
    }
}