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
        [SerializeField] private GameObject pickedObject;
        [Tooltip("Store here any interesting object for this agent")]
        [SerializeField] private List<GameObject> _objectives = new();
        public GameObject PickedObject { get => pickedObject; set => pickedObject = value; }
        public Vector3 GetPosition { get => transform.position; }
        public List<GameObject> Objectives { get => _objectives; set => _objectives = value; }
    }

}
