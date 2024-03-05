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
        private float m_chaseSpeed = 2f;

        private float m_initialSpeed = 1;
        private const float CHASE_TICK = 0.1f;
        readonly WaitForSeconds chaseCheckTick = new(CHASE_TICK);
        #endregion

        #region Methods
        // Replace Awake logic if needed
        protected internal override void Awake()
        {
            base.Awake();
            m_initialSpeed = LocalNavMeshAgent.speed;
        }

        public override void StartExecution(GameObject target = null)
        {
            base.StartExecution(target);
            StartCoroutine(Act(target));
        }
        public override void InterruptExecution()
        {
            LocalNavMeshAgent.speed = m_initialSpeed;
            base.InterruptExecution();
        }
        public override void FinishExecution()
        {
            LocalNavMeshAgent.speed = m_initialSpeed;
            base.FinishExecution();
        }
        public override List<Option> GetOptions()
        {
            // If the action can have multiple targets, you can use this implementation
            return ScoreMultipleOptions(LocalAgentMemory.HeardObjects);
        }

        protected override IEnumerator Act(GameObject target = null)
        {
            LocalNavMeshAgent.speed = m_chaseSpeed;
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
            this.m_chaseSpeed = (float)data.FindValueByName("Speed").Getvalue();
        }

        public override DataGeneric GetGeneric()
        {
            var data = base.GetGeneric();
            data.Add(new WraperNumber { name = "Speed", value = m_chaseSpeed });
            return data;
        }

        #endregion
    }
}
