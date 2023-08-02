using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ArtificialIntelligence.Utility.Actions
{
    /// <summary>
    /// Replace this summary with a description of the class.
    /// </summary>
    public class RunAway : ActionBase
    {
        #region Fields
        private float initialSpeed = 1;
        private const float RUN_TICK = 0.1f;
        WaitForSeconds runCheckTick = new(RUN_TICK);

        [SerializeField]
        private float safeDistance = 3;
        [SerializeField]
        private int runSpeed = 2;
        #endregion
        #region Properties
        
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
            return ScoreMultipleOptions(LocalAgentMemory.HeardObjects);
        }

        protected override IEnumerator Act(GameObject target = null)
        {
            Vector3 runAwayDirection = (LocalAgentMemory.GetPosition - target.transform.position).normalized;
            LocalNavMeshAgent.speed = runSpeed;
            LocalNavMeshAgent.SetDestination(LocalAgentMemory.GetPosition + runAwayDirection * safeDistance);
            while (!LocalNavMeshAgent.ReachedDestination())
            {
                yield return runCheckTick;
            }
            FinishExecution();
        }
        #endregion
    }
}
