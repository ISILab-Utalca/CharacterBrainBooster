using Generic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ArtificialIntelligence.Utility.Actions
{
    /// <summary>
    /// Replace this summary with a description of the class.
    /// </summary>
    public class Chase : ActionState
    {
        #region Fields

        [SerializeField]
        private float chaseSpeed = 2f;

        private float initialSpeed = 1;
        private const float CHASE_TICK = 0.1f;
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
            LocalNavMeshAgent.SetDestination(target.transform.position);
            while (!LocalNavMeshAgent.ReachedDestination())
            {
                yield return chaseCheckTick;
            }
            if (viewLogs) Debug.Log($"{gameObject.name} finished chasing {target.name}");
            FinishExecution();
        }

        public override void SetParams(DataGeneric data)
        {
            throw new System.NotImplementedException();
        }

        public override DataGeneric GetGeneric()
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
