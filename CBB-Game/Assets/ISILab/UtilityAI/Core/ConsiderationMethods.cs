using ArtificialIntelligence.Utility;
using ArtificialIntelligence.Utility.Actions;
using UnityEngine;

namespace dnorambu.AI.Utility
{
    [CreateAssetMenu(fileName = "General consideration methods", menuName = "Utility AI/General Methods instance")]
    public class ConsiderationMethods : ScriptableObject
    {
        public struct MethodEvaluation
        {
            public string EvaluatedVariableName;
            public float OutputValue;
        }
        public static MethodEvaluation DistanceToTarget(LocalAgentMemory agentMemory, GameObject target)
        {
            MethodEvaluation methodEvaluation;
            if (target == null)
            {
                methodEvaluation = new MethodEvaluation
                {
                    OutputValue = 0f,
                    EvaluatedVariableName = "There is no target"
                };
            }
            else
            {
                methodEvaluation = new()
                {
                    EvaluatedVariableName = "Distance to target",
                    OutputValue = Vector3.Distance(agentMemory.transform.position, target.transform.position)
                };
            }
            
            return methodEvaluation;
        }
        public static MethodEvaluation ThreatHeard(LocalAgentMemory agentMemory, GameObject target)
        {
            MethodEvaluation methodEvaluation = new()
            {
                EvaluatedVariableName = "Threat is near",
                OutputValue = agentMemory.HeardObjects.Count > 0 ? 1f : 0f,
            };
            return methodEvaluation;
        }
        public static MethodEvaluation Idle(LocalAgentMemory agentMemory, GameObject target)
        {
            MethodEvaluation methodEvaluation = new()
            {
                EvaluatedVariableName = "Constant",
                OutputValue = 0,
            };
            return methodEvaluation;
        }
        public static MethodEvaluation RoomTemperature(LocalAgentMemory agentMemory, GameObject target)
        {
            MethodEvaluation methodEvaluation = new()
            {
                EvaluatedVariableName = "Temperature",
                OutputValue = UnityEngine.Random.Range(0,0.3f),
            };
            return methodEvaluation;
        }
        public static MethodEvaluation AttackOnCooldown(LocalAgentMemory agentMemory, GameObject target)
        {
            MethodEvaluation methodEvaluation = new()
            {
                EvaluatedVariableName = "Attack cooldown",
                OutputValue = 0
            };
            if (agentMemory.gameObject.TryGetComponent(out Attack attackAction))
            {
                methodEvaluation.OutputValue = attackAction.ActionCooldown > 0 ? 1 : 0;
            }
            return methodEvaluation;

        }
    }
}

