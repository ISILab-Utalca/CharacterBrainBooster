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
        [SerializeField]
        private bool spawnAgentsOnStartup = false;
        [SerializeField]
        private int initialNumberOfAgentsToSpawn = 0;
        [Header("Agent spawnable area")]
        [SerializeField, Tooltip("Make sure that x,y coordinates are less than final point")]
        private Vector3 initialPoint;
        [SerializeField]
        private Vector3 finalPoint;
        [Header("UI Logic")]
        [SerializeField]
        private InputField agentsToSpawn;
        [SerializeField]
        private Button spawnAgentButton;
        [SerializeField]
        private Button removeAgentButton;
        [SerializeField]
        private Button removeAllAgentButton;
        [SerializeField]
        private Stack<GameObject> agents = new();

        private void Start()
        {
            if (spawnAgentsOnStartup) CreateAgents(initialNumberOfAgentsToSpawn);
            agentsToSpawn.text = "1";
            spawnAgentButton.onClick.AddListener(CreateAgents);
            removeAgentButton.onClick.AddListener(RemoveAgent);
            removeAllAgentButton.onClick.AddListener(RemoveAllAgents);
        }
        private void OnDisable()
        {
            spawnAgentButton.onClick.RemoveListener(CreateAgents);
            removeAgentButton.onClick.RemoveListener(RemoveAgent);
            removeAllAgentButton.onClick.RemoveListener(RemoveAllAgents);
        }
        private void CreateAgents(int agentCount)
        {
            while (agentCount > 0)
            {
                CreateNewAgent();
                agentCount--;
            }
        }
        private void CreateAgents()
        {
            int agentNum = int.Parse(agentsToSpawn.text);
            CreateAgents(agentNum);
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
            var newAgent = Instantiate(agentPrefab, new Vector3(xPos, yPos, zPos), Quaternion.identity);
            agents.Push(newAgent);
        }
        public void RemoveAgent()
        {
            if (agents.Count == 0) return;
            var agent = agents.Pop();
            Destroy(agent);

        }
        public void RemoveAllAgents()
        {
            while (agents.Count > 0)
            {
                RemoveAgent();
            }
            agents.Clear();
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
