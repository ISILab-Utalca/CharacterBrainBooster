using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using ArtificialIntelligence.Utility.Considerations;
using MixTheForgotten.AI;

namespace ArtificialIntelligence.Utility
{
    /// <summary>
    /// Base class for every action that the AI agent can perform.
    /// Contains common members that every action will need.
    /// </summary>
    public abstract class ActionBaseClass : MonoBehaviour, IExecutable
    {
        #region Fields
        [Header("Action general settings")]
        [SerializeField, Tooltip("Relative importance of this action with regards to other actions")]
        private protected float _actionPriority = 1f;
        [SerializeField]
        internal protected float defaultActionCooldown;
        [SerializeField, Tooltip("Do you want to see the logs of this Action?")]
        protected internal bool m_Debug = false;
        [SerializeField]
        private protected List<Consideration> _considerations = new();

        protected internal int _numberOfExecutions;
        private System.Action onFinishedAction;
        private System.Action onStartedAction;
        #endregion

        #region Properties
        public NavMeshAgent LocalNavMeshAgent { get; protected set; }
        public float ActionCooldown { get; set; }
        public LocalAgentMemory LocalAgentMemory { get; protected set; }
        public bool IsRunning { get; set; }
        public float ActionPriority { get => _actionPriority; }
        public System.Action OnFinishedAction { get => onFinishedAction; set => onFinishedAction = value; }
        public System.Action OnStartedAction { get => onStartedAction; set => onStartedAction = value; }

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

        public abstract List<Option> ScoreOptions();
        private protected float EvaluateConsiderations(GameObject target = null)
        {
            if (_considerations.Count == 0)
            {
                Debug.LogWarning($"_considerations is empty in {name}. Returning 0");
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
        /// Default implementation for scoring a single action and packaging it,
        /// along with its score and target (if any) as an executable Option instance
        /// </summary>
        /// <param name="target">The gameObject this action needs, if necessary</param>
        /// <returns>
        /// The option to be executed. If the action has scored 0, return <b>Null</b>
        /// </returns>
        protected internal Option GetScoredOption(out Option option, GameObject target = null)
        {
            // Calculate the score of this action under the current context
            float score = EvaluateConsiderations(target);

            // Create the option if the score > 0
            if (score <= 0f) { option = null; return option; }
            option = new Option(this, score, target);

            // re-scale the score
            ApplyScaleFactorToOptionScore(option);

            // Apply the relative importance (weight) of this action
            option.Score *= _actionPriority;

            return option;

        }
        protected internal List<Option> GetMultipleScoredOptions(List<GameObject> targets)
        {
            var options = new List<Option>();
            foreach (var target in targets)
            {
                GetScoredOption(out Option opt, target);
                if (opt != null) options.Add(opt);
            }
            return options.Count > 0 ? options : null;
        }
        /// <summary>
        /// Re-scale the option score based on the number of considerations
        /// this action has. This avoids that actions with more considerations always
        /// score lower than actions with fewer considerations, because of multiplication
        /// of several numbers between 0 and 1
        /// </summary>
        /// <param name="option"></param>
        private protected void ApplyScaleFactorToOptionScore(Option option)
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
            Debug.Log($"Starting execution of {GetType().Name}, number of executions: {_numberOfExecutions}");
        }
        public virtual void InterruptExecution()
        {
            IsRunning = false;
            StopAllCoroutines();
        }
        public abstract void FinishExecution();
        /// <summary>
        /// Helper function used to stop manually created coroutines
        /// </summary>
        /// <param name="c"></param>
        protected internal void ClearCoroutine(Coroutine c)
        {
            if (c != null) { StopCoroutine(c); }
            c = null;
        }
        #endregion

    }
}