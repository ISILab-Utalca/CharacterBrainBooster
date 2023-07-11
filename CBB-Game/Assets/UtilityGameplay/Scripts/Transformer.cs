using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;
namespace CBB.Lib
{
    /// <summary>
    /// Utility function for serializing data from a prefab to a Json.
    /// It looks for the prefab's name (npc type), it's associated actions
    /// and considerations for each action.
    /// </summary>
    public class Transformer : MonoBehaviour
    {
        public enum Mode { Prefab,SingleClass}
        public Mode mode = Mode.SingleClass;
        public GameObject prefab;
        
        public void PrefabToJson()
        {
            
        }
    }
}

