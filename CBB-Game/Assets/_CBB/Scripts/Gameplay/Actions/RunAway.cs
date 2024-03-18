using Generic;
using System;
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
        private float m_safeDistance = 3;
        [SerializeField]
        private int m_runSpeed = 2;

        [SerializeField, Range(0, 10)]
        private float m_pauseAfterRunning = 1f;

        private float initialSpeed = 1;
        private const float RUN_TICK = 0.1f;
        readonly WaitForSeconds runCheckTick = new(RUN_TICK);

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

            LocalNavMeshAgent.speed = m_runSpeed;
            Vector3 runAwayDirection = (LocalAgentMemory.GetPosition - target.transform.position).normalized;
            Vector3 finalDestination = LocalAgentMemory.GetPosition + runAwayDirection * m_safeDistance;
            LocalNavMeshAgent.SetDestination(finalDestination);
            while (!LocalNavMeshAgent.ReachedDestination())
            {
                yield return runCheckTick;
            }
            yield return new WaitForSeconds(m_pauseAfterRunning);
            FinishExecution();
        }

        public override void SetParams(DataGeneric data)
        {
            base.SetParams(data);
            this.m_safeDistance = (float)data.FindValueByName("Safe distance").GetValue();
            this.m_runSpeed = (int)(float)data.FindValueByName("Run speed").GetValue();
            this.m_pauseAfterRunning = (float)data.FindValueByName("Pause after running").GetValue();
        }

        public override DataGeneric GetGeneric()
        {
            var data = base.GetGeneric();
            data.Add(new WraperNumber { name = "Safe distance", value = m_safeDistance });
            data.Add(new WraperNumber { name = "Run speed", value = m_runSpeed });
            data.Add(new WraperNumber { name = "Pause after running", value = m_pauseAfterRunning });
            return data;
        }
        #endregion
    }
}