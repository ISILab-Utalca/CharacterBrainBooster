using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CBB.InternalTool
{
    public class AgentManager : MonoBehaviour
    {
        [Header("Agent configuration")]
        [SerializeField]
        private GameObject agentPrefab;
        
        [Header("Agent spawnable area")]
        [SerializeField, Tooltip("Make sure that x,y coordinates are less than final point")]
        private Vector3 initialPoint;
        [SerializeField]
        private Vector3 finalPoint;
        [Header("UI Logic")]
        
        [SerializeField]
        private Button spawnAgentButton;
        

        private void Start()
        {
            spawnAgentButton.onClick.AddListener(CreateNewAgent);
        }
        private void OnDisable()
        {
            spawnAgentButton.onClick.RemoveListener(CreateNewAgent);
        }
        
        public void CreateNewAgent()
        {
            // Choose a random point inside the area defined by the points
            var xPos = UnityEngine.Random.Range(initialPoint.x, finalPoint.x);
            var zPos = UnityEngine.Random.Range(initialPoint.z, finalPoint.z);
            // y pos is not randomly chosen since the agent has a navMeshAgent component
            // it should automatically stick to a navmesh
            var yPos = initialPoint.y;

            // Instantiate the prefab
            Instantiate(agentPrefab, new Vector3(xPos, yPos, zPos), Quaternion.identity);
        }
        
        private void OnDrawGizmosSelected()
        {
            var centerX = (initialPoint.x + finalPoint.x) / 2;
            var centerZ = (initialPoint.z + finalPoint.z) / 2;
            Vector3 center = new(centerX, transform.position.y, centerZ);
            Vector3 size = new(Mathf.Abs(initialPoint.x - finalPoint.x), .5f, Mathf.Abs(initialPoint.z - finalPoint.z));
            Gizmos.DrawWireCube(center, size);
        }
    }
}
