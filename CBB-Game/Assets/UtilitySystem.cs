using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CBB.Api
{
    public class UtilitySystem : MonoBehaviour
    {
        private void Update()
        {
            var agents = AgentObserver.Instance.Agents;
            for (int i = 0; i < agents.Count; i++)
            {
                if (agents[i].IsAvailable())
                {
                   //var action = agents.GetAction();
                    //action?.invoke();
                }
            }
        }
    }

    public class AgentObserver
    {
        private static AgentObserver instance;
        public List<Agent> Agents = new List<Agent>();

        public static AgentObserver Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AgentObserver();
                }
                return instance;
            }
        }


        public void AddAgent(Agent agent)
        {
            Agents.Add(agent);
        }

        public void RemoveAgent(Agent agent)
        {
            Agents.Remove(agent);
        }
    }
}