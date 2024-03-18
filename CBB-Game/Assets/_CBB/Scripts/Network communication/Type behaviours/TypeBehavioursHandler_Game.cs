using CBB.DataManagement;
using CBB.Lib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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
            InternalNetworkManager.OnServerMessageDequeued += ReceiveTypeBehaviours;
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
        private static void ReceiveTypeBehaviours(string json)
        {
            try
            {
                List<TypeBehaviour> typeBehaviours = JsonConvert.DeserializeObject<List<TypeBehaviour>>(json, Settings.JsonSerialization);
                //After receiving the data, the game nees to update 2 main things:
                // 1. The brain maps
                UpdateBrainMaps(typeBehaviours);
                // 2. The behaviour of the agents
                SetTypeBehaviours(typeBehaviours);
            }
            catch (Exception)
            {
                // Intentionally left empty
            }
        }

        private static void UpdateBrainMaps(List<TypeBehaviour> typeBehaviours)
        {
            // We load the current brain maps
            List<BrainMap> brainMaps = BrainMapsManager.GetAllBrainMaps();
            if (brainMaps == null) return;
            // We iterate over the received type behaviours
            foreach (var typeBehaviour in typeBehaviours)
            {
                // We find the brain map that matches the agent type
                var brainMap = brainMaps.Find(x => x.agentType == typeBehaviour.agentType);
                if (brainMap == null)
                {
                    // If the brain map does not exist, we create a new one
                    brainMap = new BrainMap(typeBehaviour.agentType);
                    brainMaps.Add(brainMap);
                }
                // We iterate over the subgroups of the received type behaviour
                foreach (var subgroup in typeBehaviour.subgroups)
                {
                    // We find the subgroup in the brain map
                    var subgroupBrain = brainMap.SubgroupsBrains.Find(x => x.subgroupName == subgroup.name);
                    if (subgroupBrain == null)
                    {
                        // If the subgroup does not exist, we create a new one
                        subgroupBrain = new BrainMap.SubgroupBrain(subgroup.name, subgroup.brainIdentification.id);
                        brainMap.SubgroupsBrains.Add(subgroupBrain);
                    }
                    else
                    {
                        // If the subgroup exists, we update the brain id
                        subgroupBrain.brainID = subgroup.brainIdentification.id;
                    }
                }
            }
            // We save the updated brain maps
            BrainMapsManager.Save(brainMaps);
        }

        private static void SetTypeBehaviours(List<TypeBehaviour> typeBehaviours)
        {
            // Load the brain maps
            List<BrainMap> brainMaps = BrainMapsManager.GetAllBrainMaps();
            if (brainMaps == null) return;
            // Get all game objects with Behaviour Loader component
            var agents = UnityEngine.Object.FindObjectsOfType<BehaviourLoader>().ToList();
            foreach (var typeBehaviour in typeBehaviours)
            {
                foreach (var subgroup in typeBehaviour.subgroups)
                {
                    foreach (var agent in subgroup.agents)
                    {
                        var agentGO = agents.Find(x => x.gameObject.GetInstanceID().ToString() == agent.id);
                        if (agentGO != null)
                        {
                            if (agentGO.TryGetComponent<BehaviourLoader>(out var behaviourLoader))
                            {
                                // Compare values before updating behaviour, to avoid unnecessary updates
                                if (behaviourLoader.m_agentType == typeBehaviour.agentType && behaviourLoader.m_agentTypeSubgroup == subgroup.name)
                                {
                                    continue;
                                }
                                behaviourLoader.m_agentType = typeBehaviour.agentType;
                                behaviourLoader.m_agentTypeSubgroup = subgroup.name;
                                // kicktart the reload behaviour process
                                behaviourLoader.UpdateBehaviour(BrainDataLoader.GetBrainByID(subgroup.brainIdentification.id));
                            }
                        }
                    }
                }
            }
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
