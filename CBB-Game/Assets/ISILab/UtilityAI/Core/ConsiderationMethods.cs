using ArtificialIntelligence.Utility;
using UnityEngine;

namespace dnorambu.AI.Utility
{
    [CreateAssetMenu(fileName = "General consideration methods", menuName = "Utility AI/General Methods instance")]
    public class ConsiderationMethods : ScriptableObject
    {
        public static float DistanceToTarget(LocalAgentMemory agentMemory, GameObject target)
        {
            return Vector3.Distance(agentMemory.transform.position, target.transform.position);
        }
        public static float ThreatHeard(LocalAgentMemory agentMemory, GameObject target)
        {
            return agentMemory.HeardObjects.Count > 0 ? 1 : 0;
        }
        public static float Idle(LocalAgentMemory agentMemory, GameObject target)
        {
            return 0;
        }
    }
}

