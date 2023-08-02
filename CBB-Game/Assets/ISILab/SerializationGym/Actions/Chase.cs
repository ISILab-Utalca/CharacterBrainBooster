using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ArtificialIntelligence.Utility.Actions
{
    /// <summary>
    /// Replace this summary with a description of the class.
    /// </summary>
    public class Chase : ActionBase
    {
        #region Fields
        private float initialSpeed = 1;
        private const float CHASE_TICK = 0.1f;
        [SerializeField]
        float chaseRange = 2f;
        [SerializeField]
        float chaseSpeed = 2f;

        readonly WaitForSeconds chaseCheckTick = new(CHASE_TICK);
        #endregion

        #region Methods
        // Replace Awake logic if needed
        protected internal override void Awake()
        {
            base.Awake();
            initialSpeed = LocalNavMeshAgent.speed;
        }

        public override void StartExecution(GameObject target = null)
        {
            base.StartExecution(target);
            StartCoroutine(Act(target));
        }
        public override void InterruptExecution()
        {
            LocalNavMeshAgent.speed = initialSpeed;
            base.InterruptExecution();
        }
        public override void FinishExecution()
        {
            LocalNavMeshAgent.speed = initialSpeed;
            base.FinishExecution();
        }
        public override List<Option> GetOptions()
        {
            // If the action can have multiple targets, you can use this implementation
            return ScoreMultipleOptions(LocalAgentMemory.HeardObjects);
        }

        protected override IEnumerator Act(GameObject target = null)
        {
            LocalNavMeshAgent.speed = chaseSpeed;
            while (HelperFunctions.TargetIsInRange(transform, target.transform, chaseRange))
            {
                LocalNavMeshAgent.SetDestination(target.transform.position);
                yield return chaseCheckTick;
            }
            if (viewLogs) Debug.Log($"{gameObject.name} finished chasing {target.name}");
            FinishExecution();
        }
        
        #endregion
    }
}
