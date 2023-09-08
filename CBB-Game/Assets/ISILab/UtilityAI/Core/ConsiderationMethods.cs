using ArtificialIntelligence.Utility;
using ArtificialIntelligence.Utility.Actions;
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
        public static float AttackOnCooldown(LocalAgentMemory agentMemory, GameObject target)
        {
            if (agentMemory.gameObject.TryGetComponent(out Attack attackAction))
            {
                return attackAction.ActionCooldown > 0 ? 1 : 0;
            }
            Debug.LogError($"Attack action not found on {agentMemory.gameObject.name}");
            return 0;
        }
    }
}

