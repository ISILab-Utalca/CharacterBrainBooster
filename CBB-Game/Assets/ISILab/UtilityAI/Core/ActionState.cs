using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MixTheForgotten.AI;
using System.Collections;

namespace ArtificialIntelligence.Utility
{
    /// <summary>
    /// Base class for any action that the AI agent can perform.
    /// </summary>
    public abstract class ActionState : MonoBehaviour, IAction
    {
        #region Fields
        [Header("Action general settings")]
        [SerializeField, Tooltip("Relative importance of this action with regards to other actions")]
        private protected float _actionPriority = 1f;
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
        [field:SerializeField]
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
            Debug.Log("Cache basic components");
            if (TryGetComponent(out LocalAgentMemory lam)) LocalAgentMemory = lam;
            if (TryGetComponent(out NavMeshAgent nma)) LocalNavMeshAgent = nma;
        }
        public abstract List<Option> GetOptions();
        protected float EvaluateConsiderations(GameObject target = null)
        {
            if (_considerations.Count == 0)
            {
                if (viewLogs) Debug.LogWarning($"_considerations is empty in {name}. Returning 0");
                return 0f;
            }
            float score = 1, considerationScore;
            foreach (var consideration in _considerations)
            {
                considerationScore = consideration.GetValue(LocalAgentMemory, target);
                // break if the score is 0, no need to compute further considerations
                if (considerationScore == 0) return 0;
                score *= considerationScore;
            }
            return score;
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
            float score = EvaluateConsiderations(target);

            option = new Option(this, score, target);

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
            if(targets.Count > 0)
            {
                foreach (var target in targets)
                {
                    ScoreSingleOption(out Option opt, target);
                    if (opt != null) options.Add(opt);
                }
            }
            else
            {
                options.Add(new Option(this,0,null));
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
            option.Score = originalScore + value * originalScore;
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
            c = null;
        }
        protected virtual IEnumerator Act(GameObject target = null)
        {
            yield return null;
        }
        #endregion
    }
}