using ArtificialIntelligence.Utility.Actions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace ArtificialIntelligence.Utility
{
    /// <summary>
    /// Observes changes on sensors and calls the Utility system
    /// to start new behaviours
    /// </summary>
    
    public class AgentBrain : MonoBehaviour, IAgentBrain
    {
        [SerializeField, Tooltip("The brain will tell the Utility System class to pick an option based on this heuristic")]
        private UtilityDecisionMaker.PickMethod _pickMethod;
        [SerializeField, Range(0, 5), Tooltip("If Pick method is top N, define how many options to consider")]
        private int topN = 1;
        [Tooltip("The actions that this agent can perform")]
        [SerializeField]
        private List<IAction> _actions = new();

        [Tooltip("Default action that this agent will execute if all are scored to 0")]
        [SerializeField]
        private ActionState _defaultAction;

        [SerializeField]
        private bool viewLogs = false;
        private ActionRunner _actionRunner;

        public BubbleText panelText;

        public System.Action<List<Option>> OnCompletedScoring { get; set; }
        public System.Action OnSetupDone { get; set; }
        public System.Action<Option, List<Option>> OnDecisionTaken { get; set; }
        public System.Action<ISensor> OnSensorUpdate { get; set; }

        public List<ISensor> Sensors { get; private set; }

        // Get references to the action runner and all sensors and actions on the agent
        private void Awake()
        {
            _actionRunner = gameObject.AddComponent<ActionRunner>();
            Sensors = gameObject.GetComponentsInChildren<ISensor>().ToList();
            _actions.AddRange(gameObject.GetComponents<IAction>());

            var panel = Instantiate(panelText, panelText.Canvas.GetComponent<RectTransform>());
            panel.Init(this.gameObject.transform);

        }
        // Subscribe to sensor updates and finished action events
        private void OnEnable()
        {
            SubscribeToSensors(Sensors);
            _actionRunner.OnFinishedExecution += TryStarNewActionOnFinish;
            OnSetupDone?.Invoke();
        }
        // Unsubscribe from sensor updates and finished action events
        private void OnDisable()
        {
            UnsubscribeFromSensors(Sensors);
            _actionRunner.OnFinishedExecution -= TryStarNewActionOnFinish;
        }

        public void TryStarNewActionOnFinish()
        {
            TryStartNewAction(null);
        }

        private void Start()
        {
            // Begin the life of this agent
            TryStartNewAction(null);
        }
        public void TryStartNewAction(ISensor sensor)
        {
            Option newOption = GetNewOption();
            if (newOption != null && newOption.Score != 0)
            {
                _actionRunner.ExecuteOption(newOption);
            }
            else
            {
                // Execute an Idle action just to keep alive the Sense - Think - Act cycle
                if (viewLogs) Debug.LogWarning("Executing default action on:" + gameObject.name);
                newOption = new Option();
                if(TryGetComponent(out Idle idleAction))
                {
                    newOption.Action = idleAction;
                }
                else
                {
                    newOption.Action  = gameObject.AddComponent<Idle>();
                }
                _actionRunner.ExecuteOption(newOption);
            }
        }
        private Option GetNewOption()
        {
            List<Option> scoredOptions = UtilityDecisionMaker.GetPossibleOptions(_actions);
            OnCompletedScoring?.Invoke(scoredOptions);
            var bestOption = UtilityDecisionMaker.PickFromPossibleOptions(scoredOptions, _pickMethod, topN);
            scoredOptions.Remove(bestOption);
            OnDecisionTaken?.Invoke(bestOption, scoredOptions);
            return bestOption;
        }
        private void SubscribeToSensors(List<ISensor> sensors)
        {
            foreach (ISensor sensor in sensors)
            {
                sensor.OnSensorUpdate += TryStartNewAction;
                sensor.OnSensorUpdate += OnSensorUpdate;
            }
        }
        private void UnsubscribeFromSensors(List<ISensor> sensors)
        {
            foreach (ISensor sensor in sensors)
            {
                sensor.OnSensorUpdate -= TryStartNewAction;
                sensor.OnSensorUpdate -= OnSensorUpdate;
            }
        }
    }
}