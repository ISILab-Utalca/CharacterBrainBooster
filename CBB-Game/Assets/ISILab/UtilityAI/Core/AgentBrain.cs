using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace ArtificialIntelligence.Utility
{
    /// <summary>
    /// Observes changes on sensors and calls the Utility system
    /// to start new behaviours
    /// </summary>
    [RequireComponent(typeof(ActionRunner))]
    public class AgentBrain : MonoBehaviour, IAgentBrain
    {
        [SerializeField, Tooltip("The brain will tell the Utility System class to pick an option based on this heuristic")]
        private UtilityDecisionMaker.PickMethod _pickMethod;
        [SerializeField, Range(0, 5), Tooltip("If Pick method is top N, define how many options to consider")]
        private int topN = 1;
        [Tooltip("The actions that this agent can perform")]
        [SerializeField]
        private List<ActionState> _actions = new();

        [Tooltip("Default action that this agent will execute if all are scored to 0")]
        [SerializeField]
        private ActionState _defaultAction;

        [SerializeField]
        private bool viewLogs = false;
        private ActionRunner _actionRunner;

        public System.Action<List<Option>> OnCompletedScoring { get; set; }
        public System.Action OnSetupDone { get; set; }
        public System.Action<Option, List<Option>> OnDecisionTaken { get; set; }
        
        public List<ISensor> Sensors { get; private set; }

        // Get references to the action runner and all sensors and actions on the agent
        private void Awake()
        {
            _actionRunner = GetComponent<ActionRunner>();
            Sensors = gameObject.GetComponentsInChildren<ISensor>().ToList();
            _actions.AddRange(gameObject.GetComponents<ActionState>());

        }
        // Subscribe to sensor updates and finished action events
        private void OnEnable()
        {
            SubscribeToSensors(Sensors);
            _actionRunner.OnFinishedExecution += TryStartNewAction;
            OnSetupDone?.Invoke();
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
            var bestOption = UtilityDecisionMaker.PickFromScoredOptions(scoredOptions, _pickMethod, topN);
            scoredOptions.Remove(bestOption);
            OnDecisionTaken?.Invoke(bestOption, scoredOptions);
            return bestOption;
        }
        private void SubscribeToSensors(List<ISensor> sensors)
        {
            foreach (ISensor sensor in sensors)
            {
                sensor.OnSensorUpdate += TryStartNewAction;
            }
        }
        private void UnsubscribeFromSensors(List<ISensor> sensors)
        {
            foreach (ISensor sensor in sensors)
            {
                sensor.OnSensorUpdate -= TryStartNewAction;
            }
        }
    }
}