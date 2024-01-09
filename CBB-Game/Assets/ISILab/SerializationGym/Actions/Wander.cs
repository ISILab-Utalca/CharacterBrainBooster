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
        private float _minWaitTimer = 1f;
        [SerializeField, Tooltip("Max time the agent will wait after reaching destination")]
        private float _maxWaitTimer = 5f;
        //private float _minDistanceDelta = 1f;
        [SerializeField, Tooltip("Radius where an agent will walk to, from its current position")]
        private float _walkRadius = 10f;
        [SerializeField, Tooltip("How often the script will check if the agent reached its destination")]
        private float _tickCheck = .2f;

        private WaitForSeconds _distanceCheckTime;
        private Vector3 _randomDirection;

        #endregion

        #region Methods
        protected internal override void Awake()
        {
            base.Awake();
            _distanceCheckTime = new WaitForSeconds(_tickCheck);
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
            _randomDirection = Random.insideUnitSphere * _walkRadius + transform.position;
            // Sometimes random direction could be out of the navmesh, so we calculate the closest point
            // LayerMask = -1 param is recomended to hit any layer (same as Navmesh.AllAreas)
            if (NavMesh.SamplePosition(_randomDirection, out NavMeshHit navHit, _walkRadius, NavMesh.AllAreas))
            {
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
            yield return new WaitForSeconds(Random.Range(_minWaitTimer, _maxWaitTimer));
            FinishExecution();
        }

        public override void SetParams(DataGeneric data)
        {
            base.SetParams(data);
            this._minWaitTimer = (float) data.FindValueByName("MinWaitTimer").Getvalue();
            this._maxWaitTimer = (float) data.FindValueByName("MaxWaitTimer").Getvalue();
            this._walkRadius = (float) data.FindValueByName("WalkRadius").Getvalue();
            this._tickCheck = (float) data.FindValueByName("TickCheck").Getvalue();
        }
        public override DataGeneric GetGeneric()
        {
            var data = new DataGeneric() { ClassType = typeof(Wander) };
            data.Add(new WraperNumber { name = "MinWaitTimer", value = _minWaitTimer });
            data.Add(new WraperNumber { name = "MaxWaitTimer", value = _maxWaitTimer });
            data.Add(new WraperNumber { name = "WalkRadius", value = _walkRadius });
            data.Add(new WraperNumber { name = "TickCheck", value = _tickCheck });
            AddConsiderationsToConfiguration(data);
            return data;
        }
        #endregion

    }
}