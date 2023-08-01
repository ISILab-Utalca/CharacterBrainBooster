using System.Collections.Generic;
using UnityEngine;

namespace ArtificialIntelligence.Utility
{
    /// <summary>
    /// Class that holds general Agent's properties that represents its current
    /// information about the world.
    /// </summary>
    public class LocalAgentMemory : MonoBehaviour
    {
        [SerializeField,Tooltip("Store here any interesting object for this agent")]
        private List<GameObject> _objectives = new();
        private List<GameObject> heardObjects = new();
        public Vector3 GetPosition { get => transform.position; }
        public List<GameObject> Objectives { get => _objectives; set => _objectives = value; }
        public List<GameObject> HeardObjects { get => heardObjects; set => heardObjects = value; }
    }

}
