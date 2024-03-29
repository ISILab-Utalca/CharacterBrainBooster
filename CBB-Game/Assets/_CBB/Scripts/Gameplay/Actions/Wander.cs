using Generic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace ArtificialIntelligence.Utility.Actions
{
    public class Wander : ActionState
    {
        #region Fields
        [SerializeField, Tooltip("Min time the agent will wait after reaching destination")]
        private float m_minWaitTimer = 1f;
        [SerializeField, Tooltip("Max time the agent will wait after reaching destination")]
        private float m_maxWaitTimer = 5f;
        [SerializeField, Tooltip("Radius where an agent will walk to, from its current position")]
        private float m_walkRadius = 10f;
        [SerializeField, Tooltip("How often the script will check if the agent reached its destination")]
        private float m_tickCheck = .2f;
        [SerializeField]
        private float m_speed = 5;
        private WaitForSeconds _distanceCheckTime;
        private Vector3 _randomDirection;

        #endregion

        #region Methods
        protected internal override void Awake()
        {
            base.Awake();
            _distanceCheckTime = new WaitForSeconds(m_tickCheck);
        }
        public override void StartExecution(GameObject target = null)
        {
            base.StartExecution();
            StartCoroutine(Act());
        }
        public override void InterruptExecution()
        {
            base.InterruptExecution();
        }
        public override void FinishExecution()
        {
            base.FinishExecution();
        }
        public override List<Option> GetOptions()
        {
            return ScoreSingleOption(out Option option) != null ? new List<Option> { option } : null;
        }
        protected override IEnumerator Act(GameObject target = null)
        {
            _randomDirection = Random.insideUnitSphere * m_walkRadius + transform.position;
            // Sometimes random direction could be out of the navmesh, so we calculate the closest point
            // LayerMask = -1 param is recomended to hit any layer (same as Navmesh.AllAreas)
            if (NavMesh.SamplePosition(_randomDirection, out NavMeshHit navHit, m_walkRadius, NavMesh.AllAreas))
            {
                LocalNavMeshAgent.speed = m_speed;
                LocalNavMeshAgent.SetDestination(navHit.position);
                //if (LocalNavMeshAgent.isOnNavMesh) Debug.Log("Yes is on Navmesh");
                while (!LocalNavMeshAgent.ReachedDestination())
                {
                    LocalNavMeshAgent.SetDestination(navHit.position);
                    yield return _distanceCheckTime;
                }

            }
            // If no random point is found, halt for a second
            else
            {
                if (viewLogs) Debug.Log("No random point found");
                yield return new WaitForSeconds(1f);
            }
            if (viewLogs) Debug.Log("Reached destination. Waiting ...");
            yield return new WaitForSeconds(Random.Range(m_minWaitTimer, m_maxWaitTimer));
            FinishExecution();
        }

        public override void SetParams(DataGeneric data)
        {
            base.SetParams(data);
            this.m_minWaitTimer = (float) data.FindValueByName("Min wait timer").GetValue();
            this.m_maxWaitTimer = (float) data.FindValueByName("Max wait timer").GetValue();
            this.m_walkRadius = (float) data.FindValueByName("Walk radius").GetValue();
            this.m_tickCheck = (float) data.FindValueByName("Tick check").GetValue();
            this.m_speed = (float) data.FindValueByName("Agent speed").GetValue();
        }
        public override DataGeneric GetGeneric()
        {
            var data = base.GetGeneric();
            data.Add(new WraperNumber { name = "Min wait timer", value = m_minWaitTimer });
            data.Add(new WraperNumber { name = "Max wait timer", value = m_maxWaitTimer });
            data.Add(new WraperNumber { name = "Walk radius", value = m_walkRadius });
            data.Add(new WraperNumber { name = "Tick check", value = m_tickCheck });
            data.Add(new WraperNumber { name = "Agent speed", value = m_speed });
            return data;
        }
        #endregion

    }
}