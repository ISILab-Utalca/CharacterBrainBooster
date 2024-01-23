using CBB.Lib;
using Generic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace ArtificialIntelligence.Utility
{
    /// <summary>
    /// Base class for any action that the AI agent can perform.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent),typeof(LocalAgentMemory))]
    public abstract class ActionState : MonoBehaviour, IAction, IGeneric
    {
        #region Fields
        [Header("Action general settings")]
        [SerializeField, Tooltip("Relative importance of this action with regards to other actions")]
        public float _actionPriority = 1f;
        [SerializeField]
        internal protected float defaultActionCooldown;
        [SerializeField, Tooltip("Do you want to see the logs of this Action?")]
        protected internal bool viewLogs = false;
        [SerializeField]
        private protected List<UtilityConsideration> _considerations = new();

        protected internal int _numberOfExecutions;
        #endregion

        #region Properties
        public NavMeshAgent LocalNavMeshAgent { get; protected set; }
        [field: SerializeField]
        public float ActionCooldown { get; set; }
        public LocalAgentMemory LocalAgentMemory { get; protected set; }
        public bool IsRunning { get; set; }
        public float ActionPriority { get => _actionPriority; }
        public System.Action OnFinishedAction { get; set; }
        public System.Action OnStartedAction { get; set; }
        public bool IsBlocked { get; protected set; }
        #endregion

        #region Methods
        protected internal virtual void Awake()
        {
            // Warning: this assumes that the agent has a NavMeshAgent component
            // and a LocalAgentMemory component at the same level in the hierarchy
            // TODO: make this more robust
            //Debug.Log("Cache basic components");
            LocalAgentMemory = GetComponent<LocalAgentMemory>();
            LocalNavMeshAgent = GetComponent<NavMeshAgent>();
        }
        public abstract List<Option> GetOptions();
        protected Option EvaluateConsiderations(GameObject target = null)
        {
            Option option = new();
            if (_considerations.Count == 0) return option;
            
            float score = 1;
            UtilityConsideration.Evaluation evaluation;
            foreach (var consideration in _considerations)
            {
                //Note: is possible to optimize this, breaking after a consideration return 0
                // but if we do that, the debugging tool won't have information about the next
                // considerations(that may have scores different than 0), so we compute all
                // the considerations, regardless of their evaluated value
                evaluation = consideration.GetValue(LocalAgentMemory, target);
                option.Evaluations.Add(evaluation);

                //This multiplication is where we combine the scores of the different
                //considerations being evaluated.This line of code is very important since it
                //is responsible for combining the different evaluation curves into a single uniform value.
                score *= evaluation.UtilityValue;
            }
            option.Score = score;
            return option;
        }
        /// <summary>
        /// Evaluate the action against the agent itself, a fixed target or no target at all.
        /// </summary>
        /// <param name="target">The gameObject this action needs, if necessary</param>
        /// <returns>
        /// The option to be executed.
        /// </returns>
        protected internal Option ScoreSingleOption(out Option option, GameObject target = null)
        {
            option = EvaluateConsiderations(target);
            option.Action = this;
            option.Target = target;
            RescaleOptionScore(option);

            // Apply the relative importance (weight) of this action
            option.Score *= _actionPriority;

            return option;

        }
        /// <summary>
        /// Evaluate the action against several targets
        /// </summary>
        /// <param name="targets"></param>
        /// <returns>All the available options</returns>
        protected internal List<Option> ScoreMultipleOptions(List<GameObject> targets)
        {
            var options = new List<Option>();
            if (targets.Count > 0)
            {
                foreach (var target in targets)
                {
                    ScoreSingleOption(out Option opt, target);
                    options.Add(opt);
                }
            }
            else if (targets.Count == 0)
            {
                //Create an option to debug, although this action had no target.
                var opt = new Option(this);
                foreach(var consideration in this._considerations)
                {
                    var eval = consideration.GetValue(LocalAgentMemory, null);
                    eval.UtilityValue = 0;
                    opt.Evaluations.Add(eval);
                }
                options.Add(opt);
            }

            return options;
        }
        /// <summary>
        /// Re-scale the option score based on the number of considerations
        /// this option's action has. This avoids that actions with more considerations always
        /// score lower than actions with fewer considerations, because of multiplication
        /// of several values between 0 and 1
        /// </summary>
        /// <param name="option"></param>
        protected void RescaleOptionScore(Option option)
        {
            // Debug score before
            float originalScore = option.Score;
            float modification = 1f - 1f / _considerations.Count;
            float value = (1f - originalScore) * modification;
            option.Score = originalScore * (1 + value);
            option.ScaleFactor = (1 + value);
        }

        public virtual void StartExecution(GameObject target = null)
        {
            IsRunning = true;
            _numberOfExecutions++;
            if (viewLogs) Debug.Log($"Starting execution of {GetType().Name}, number of executions: {_numberOfExecutions}");
        }
        public virtual void InterruptExecution()
        {
            IsRunning = false;
            StopAllCoroutines();
        }
        /// <summary>
        /// Call this method whenever the action reaches a state where it can't be executed,
        /// has to be interrupted (by other action) or finished normally.
        /// </summary>
        public virtual void FinishExecution()
        {
            IsRunning = false;
            if (viewLogs) Debug.Log($"Finish execution of {GetType().Name}");
            OnFinishedAction?.Invoke();
        }
        /// <summary>
        /// Helper function used to stop manually created coroutines
        /// </summary>
        /// <param name="c"></param>
        protected void ClearCoroutine(Coroutine c)
        {
            if (c != null) { StopCoroutine(c); }
        }
        protected virtual IEnumerator Act(GameObject target = null)
        {
            yield return null;
        }

        public List<UtilityConsideration> GetConsiderations()
        {
            return _considerations;
        }
        public void AddConsiderationsToConfiguration(DataGeneric config)
        {
            foreach (var item in _considerations)
            {
                config.Add(new WrapperConsideration()
                {
                    name = item.considerationName,
                    configuration = item.GetConfiguration()
                });
            }
        }
        public virtual void SetParams(DataGeneric data)
        {
            SetConsiderationsFromConfiguration(data);
        }
        internal void SetConsiderationsFromConfiguration(DataGeneric data)
        {
            _considerations.Clear();

            foreach (var value in data.Values)
            {
                if (value is WrapperConsideration wc)
                {
                    UtilityConsideration consideration = new();

                    consideration.SetParamsFromConfiguration(wc.configuration);
                    _considerations.Add(consideration);
                }
            }
        }
        public abstract DataGeneric GetGeneric();
        #endregion
    }
}