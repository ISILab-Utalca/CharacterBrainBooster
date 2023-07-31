using System.Collections.Generic;
using UnityEngine;
namespace ArtificialIntelligence.Utility
{
    /// <summary>
    /// Observes changes on sensors and calls the Utility system
    /// to start new behaviours
    /// </summary>
    [RequireComponent(typeof(ActionRunner))]
    public class AgentBrain : MonoBehaviour
    {
        [Tooltip("The brain will tell the Utility System class to pick an action based on this heuristic")]
        [SerializeField] private UtilityDecisionMaker.PickMethod _pickMethod;

        public List<SensorBaseClass> _sensors;

        [Tooltip("The actions that this agent can perform")]
        [SerializeField] private List<ActionBase> _actions = new();
        private ActionRunner _actionRunner;

        [Tooltip("Default action that this agent will execute if all are scored to 0")]
        [SerializeField]
        private ActionBase _defaultAction;

        private System.Action<List<Option>> onCompletedScoring;

        public System.Action<List<Option>> OnCompletedScoring { get => onCompletedScoring; set => onCompletedScoring = value; }

        // Get references to the action runner and all sensors and actions on the agent
        private void Awake()
        {
            _actionRunner = GetComponent<ActionRunner>();
            _sensors = gameObject.GetComponentsOnHierarchy<SensorBaseClass>();
            _actions.AddRange(gameObject.GetComponentsOnHierarchy<ActionBase>());

        }
        // Subscribe to sensor updates and finished action events
        private void OnEnable()
        {
            SubscribeToSensors(_sensors);
            _actionRunner.OnFinishedExecution += TryStartNewAction;
        }
        // Unsubscribe from sensor updates and finished action events
        private void OnDisable()
        {
            UnsubscribeFromSensors(_sensors);
            _actionRunner.OnFinishedExecution -= TryStartNewAction;
        }
        private void Start()
        {
            // Begin the life of this agent
            TryStartNewAction();
        }
        public void TryStartNewAction()
        {
            Option newOption = GetNewOption();
            if (newOption != null)
            {
                _actionRunner.TryExecuteOption(newOption);
            }
            else
            {
                // Since we shouldn't (in most cases) fix a target for the default action in Editor mode,
                // it needs to be a self-targeted action (like a bark, simple idle animation, etc)
                // TODO: add a Custom Editor that shows this warning
                Debug.LogWarning("Executing default action on:" + gameObject.name);
                newOption = new Option(_defaultAction, 1, null);
                _actionRunner.TryExecuteOption(newOption);
            }
        }
        private Option GetNewOption()
        {
            // First, update the score of every action the agent can perform on this frame
            List<Option> scoredOptions = UtilityDecisionMaker.ScorePossibleOptions(_actions);
            OnCompletedScoring?.Invoke(scoredOptions);
            // Then return the action with best score (by default)
            var bestOption = UtilityDecisionMaker.PickFromScoredOptions(scoredOptions, _pickMethod);
            return bestOption;
        }
        private void SubscribeToSensors(List<SensorBaseClass> sensors)
        {
            foreach (SensorBaseClass sensor in sensors)
            {
                sensor.OnSensorUpdate += TryStartNewAction;
            }
        }
        private void UnsubscribeFromSensors(List<SensorBaseClass> sensors)
        {
            foreach (SensorBaseClass sensor in sensors)
            {
                sensor.OnSensorUpdate -= TryStartNewAction;
            }
        }
    }
}