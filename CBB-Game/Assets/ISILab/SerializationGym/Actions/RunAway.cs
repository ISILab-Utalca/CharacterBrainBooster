using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ArtificialIntelligence.Utility.Actions
{
    /// <summary>
    /// Escape from a threat and save the agent's life
    /// </summary>
    public class RunAway : ActionState
    {
        #region Fields
        [SerializeField]
        private float safeDistance = 3;
        [SerializeField]
        private int runSpeed = 2;

        [SerializeField]
        private float pauseAfterRunning = 1f;

        private float initialSpeed = 1;
        private const float RUN_TICK = 0.1f;
        WaitForSeconds runCheckTick = new(RUN_TICK);

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

            LocalNavMeshAgent.speed = runSpeed;
            if (viewLogs) Debug.LogWarning($"CAUTION: LOCKING ACTION EXECUTION ON {gameObject.name}.\nLocked action: {this}");
            IsBlocked = true;
            Vector3 runAwayDirection = (LocalAgentMemory.GetPosition - target.transform.position).normalized;
            Vector3 finalDestination = LocalAgentMemory.GetPosition + runAwayDirection * safeDistance;
            LocalNavMeshAgent.SetDestination(finalDestination);
            while (!LocalNavMeshAgent.ReachedDestination())
            {
                yield return runCheckTick;
            }
            yield return new WaitForSeconds(pauseAfterRunning);
            IsBlocked = false;
            if (viewLogs) Debug.LogWarning($"CAUTION: UNLOCKING ACTION EXECUTION ON {gameObject.name}.\nUnlocked action: {this}");
            if (viewLogs) Debug.Log($"Run away finished on {gameObject.name}");
            FinishExecution();
        }
        #endregion
    }
}