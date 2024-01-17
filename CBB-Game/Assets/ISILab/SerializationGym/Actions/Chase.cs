using Generic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ArtificialIntelligence.Utility.Actions
{
    /// <summary>
    /// Basic behaviour of chasing a target that has a certain tag
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
            base.SetParams(data);
            this.chaseSpeed = (float)data.FindValueByName("chaseSpeed").Getvalue();
            this.initialSpeed = (float)data.FindValueByName("initialSpeed").Getvalue();

        }

        public override DataGeneric GetGeneric()
        {
            var data = new DataGeneric(DataGeneric.DataType.Action) { ClassType = GetType() };
            data.Add(new WraperNumber { name = "chaseSpeed", value = chaseSpeed });
            data.Add(new WraperNumber { name = "initialSpeed", value = initialSpeed });
            AddConsiderationsToConfiguration(data);
            return data;
        }

        #endregion
    }
}
