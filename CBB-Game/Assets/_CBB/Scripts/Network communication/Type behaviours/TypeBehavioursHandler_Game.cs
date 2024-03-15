using CBB.DataManagement;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

namespace CBB.Comunication
{
    public static class TypeBehavioursHandler_Game
    {
        //TODO: Handle incoming type behaviours

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init()
        {
            Server.OnNewClientConnected += SendTypeBehaviours;
        }

        private static void SendTypeBehaviours(TcpClient client)
        {
            List<BrainMap> brainMaps = BrainMapsManager.GetAllBrainMaps();
            List<TypeBehaviour> blobs = new();
            if (brainMaps == null) return;
            foreach (var brainMap in brainMaps)
            {
                var assignedBehaviours = new TypeBehaviour(brainMap.agentType);
                foreach (var subgroup in brainMap.SubgroupsBrains)
                {
                    var SubgroupsBehaviour = new SubgroupBehaviour(subgroup.subgroupName);
                    var brain = BrainDataLoader.GetBrainByID(subgroup.brainID);
                    SubgroupsBehaviour.SetBrainIdentification(brain);
                    // Find all gameobjects with Behaviour Loader component
                    var agents = GameObject.FindObjectsOfType<BehaviourLoader>();
                    foreach (var agent in agents)
                    {
                        string agentType = agent.m_agentType;
                        string agentSubgroup = agent.m_agentTypeSubgroup;
                        if (agentType == brainMap.agentType && agentSubgroup == subgroup.subgroupName)
                        {
                            SubgroupsBehaviour.agents.Add(new AgentIdentification
                            {
                                name = agent.name,
                                id = agent.gameObject.GetInstanceID().ToString()
                            });
                        }
                    }
                    assignedBehaviours.subgroups.Add(SubgroupsBehaviour);
                }
                blobs.Add(assignedBehaviours);
            }

            string json = JsonConvert.SerializeObject(blobs, Settings.JsonSerialization);
            Server.SendMessageToClient(client, json);
        }

    }
    [System.Serializable]
    public class TypeBehaviour
    {
        public string agentType;
        public List<SubgroupBehaviour> subgroups;
        public TypeBehaviour(string agentType)
        {
            this.agentType = agentType;
            subgroups = new List<SubgroupBehaviour>();
        }
    }
    [System.Serializable]
    public class SubgroupBehaviour
    {
        public string name;
        public BrainIdentification brainIdentification;
        public List<AgentIdentification> agents;
        public SubgroupBehaviour(string subgroupName)
        {
            this.name = subgroupName;
            brainIdentification = null;
            agents = new List<AgentIdentification>();
        }
        public void SetBrainIdentification(Brain brain)
        {
            brainIdentification = brain.GetBrainIdentification();
        }
        public void AddAgent(AgentIdentification agent)
        {
            agents.Add(agent);
        }
        public void RemoveAgent(AgentIdentification agent)
        {
            agents.Remove(agent);
        }
    }
    [System.Serializable]
    public class BrainIdentification
    {
        public string name;
        public string id;
        public BrainIdentification(string id, string name)
        {
            this.id = id;
            this.name = name;
        }
    }
    [System.Serializable]
    public class AgentIdentification
    {
        public string name;
        public string id;
    }
}