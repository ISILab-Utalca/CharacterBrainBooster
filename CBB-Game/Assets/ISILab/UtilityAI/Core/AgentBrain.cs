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

        [Tooltip("The actions that this agent can perform")]
        [SerializeField]
        private List<ActionState> _actions = new();

        [Tooltip("Default action that this agent will execute if all are scored to 0")]
        [SerializeField]
        private ActionState _defaultAction;

        [SerializeField]
        private bool viewLogs = false;
        private System.Action<List<Option>> onCompletedScoring;
        private ActionRunner _actionRunner;
        private List<Sensor> sensors;

        public System.Action<List<Option>> OnCompletedScoring { get => onCompletedScoring; set => onCompletedScoring = value; }
        public List<Sensor> Sensors { get => sensors; set => sensors = value; }

        // Get references to the action runner and all sensors and actions on the agent
        private void Awake()
        {
            _actionRunner = GetComponent<ActionRunner>();
            Sensors = gameObject.GetComponentsOnHierarchy<Sensor>();
            _actions.AddRange(gameObject.GetComponentsOnHierarchy<ActionState>());

        }
        // Subscribe to sensor updates and finished action events
        private void OnEnable()
        {
            SubscribeToSensors(Sensors);
            _actionRunner.OnFinishedExecution += TryStartNewAction;
        }
        // Unsubscribe from sensor updates and finished action events
        private void OnDisable()
        {
            UnsubscribeFromSensors(Sensors);
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
                _actionRunner.ExecuteOption(newOption);
            }
            else
            {
                // Since we shouldn't (in most cases) fix a target for the default action in Editor mode,
                // it needs to be a self-targeted action (like a bark, simple idle animation, etc)
                // TODO: add a Custom Editor that shows this warning
                if (viewLogs) Debug.LogWarning("Executing default action on:" + gameObject.name);
                newOption = new Option(_defaultAction, 1, null);
                _actionRunner.ExecuteOption(newOption);
            }
        }
        private Option GetNewOption()
        {
            List<Option> scoredOptions = UtilityDecisionMaker.ScorePossibleOptions(_actions);
            OnCompletedScoring?.Invoke(scoredOptions);
            var bestOption = UtilityDecisionMaker.PickFromScoredOptions(scoredOptions, _pickMethod);
            return bestOption;
        }
        private void SubscribeToSensors(List<Sensor> sensors)
        {
            foreach (Sensor sensor in sensors)
            {
                sensor.OnSensorUpdate += TryStartNewAction;
            }
        }
        private void UnsubscribeFromSensors(List<Sensor> sensors)
        {
            foreach (Sensor sensor in sensors)
            {
                sensor.OnSensorUpdate -= TryStartNewAction;
            }
        }
    }
}