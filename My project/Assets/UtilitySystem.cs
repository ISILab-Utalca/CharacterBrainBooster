using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CBB.Api
{
    public class UtilitySystem : MonoBehaviour
    {

    }

    public class AgentDesitionTreee
    {
        // AgentDesitionTreee continuar trabajando
    }

    public class AgentObserver
    {
        private static AgentObserver instance;
        public List<Agent> AllMinions = new List<Agent>();

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
            AllMinions.Add(agent);
        }

        public void RemoveAgent(Agent agent)
        {
            AllMinions.Remove(agent);
        }
    }
}