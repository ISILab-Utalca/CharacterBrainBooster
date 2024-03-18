using ArtificialIntelligence.Utility.Actions;
using CBB.Lib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace ArtificialIntelligence.Utility
{
    /// <summary>
    /// Gives an agent its capacity to think and take decisions
    /// </summary>
    [RequireComponent(typeof(BehaviourLoader))]
    public class AgentBrain : MonoBehaviour, IAgentBrain
    {
        [SerializeField, Tooltip("The brain will tell the Utility System class to pick an option based on this heuristic")]
        private UtilityDecisionMaker.PickMethod _pickMethod;
        [SerializeField, Range(0, 5), Tooltip("If Pick method is top N, define how many options to consider")]
        private int topN = 1;
        [Tooltip("Default action that this agent will execute if all are scored to 0")]
        [SerializeField]
        private ActionState _defaultAction;

        [SerializeField]
        private bool viewLogs = false;
        private ActionRunner m_actionRunner;

        [SerializeField]
        private bool m_isPaused = false;
        public UtilityDecisionMaker.PickMethod PickMethod { get => _pickMethod; set => _pickMethod = value; }
        public int TopN { get => topN; set => topN = value; }

        public System.Action<List<Option>> OnCompletedScoring { get; set; }
        public System.Action<Option, List<Option>> OnDecisionTaken { get; set; }
        public System.Action<SensorActivation> OnSensorUpdate { get; set; }

        [field: SerializeField]
        public List<ISensor> Sensors { get; private set; } = new();
        [field: SerializeField]
        public List<IAction> Actions { get; private set; } = new();

        private void Awake()
        {
            m_actionRunner = gameObject.AddComponent<ActionRunner>();
            m_actionRunner.OnFinishedExecution += TryStartNewAction;

        }
        
        // Unsubscribe from sensor updates and finished action events
        private void OnDisable()
        {
            UnsubscribeFromSensors(Sensors);
            m_actionRunner.OnFinishedExecution -= TryStartNewAction;
        }

        private void StartNewActionAfterSensorActivation(SensorActivation sensorActivation)
        {
            TryStartNewAction();
        }
        public void TryStartNewAction()
        {
            if (m_isPaused) return;
            Option newOption = GetNewOption();
            if (newOption != null && newOption.Score != 0)
            {
                m_actionRunner.ExecuteOption(newOption);
            }
            else
            {
                // Execute an Idle action just to keep alive the Sense - Think - Act cycle
                if (viewLogs) Debug.LogWarning("Executing default action on:" + gameObject.name);
                newOption = new Option();
                if (TryGetComponent(out Idle idleAction))
                {
                    newOption.Action = idleAction;
                }
                else
                {
                    newOption.Action = gameObject.AddComponent<Idle>();
                }
                m_actionRunner.ExecuteOption(newOption);
            }
        }
        private Option GetNewOption()
        {
            List<Option> scoredOptions = UtilityDecisionMaker.GetPossibleOptions(Actions);
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
                sensor.OnSensorUpdate += StartNewActionAfterSensorActivation;
                sensor.OnSensorUpdate += OnSensorUpdate;
            }
        }
        private void UnsubscribeFromSensors(List<ISensor> sensors)
        {
            foreach (ISensor sensor in sensors)
            {
                sensor.OnSensorUpdate -= StartNewActionAfterSensorActivation;
                sensor.OnSensorUpdate -= OnSensorUpdate;
            }
        }
        /// <summary>
        /// Pause the behaviour of this agent, i.e. stops thinking and taking decisions
        /// </summary>
        public void Pause()
        {
            m_isPaused = true;
        }
        /// <summary>
        /// Resume the behaviour of this agent, i.e. starts thinking and taking decisions
        /// </summary>
        public void Resume()
        {
            m_isPaused = false;
            ReloadBehaviours();
            TryStartNewAction();
        }
        public void ReloadBehaviours()
        {
            ReloadActions();
            ReloadSensors();
        }
        private void ReloadSensors()
        {
            UnsubscribeFromSensors(Sensors);
            Sensors = gameObject.GetComponentsInChildren<ISensor>().ToList();
            SubscribeToSensors(Sensors);
        }
        private void ReloadActions()
        {
            Actions = gameObject.GetComponentsInChildren<IAction>().ToList();
        }
        public bool IsRunningAction()
        {
            return m_actionRunner.IsRunning;
        }
    }
}