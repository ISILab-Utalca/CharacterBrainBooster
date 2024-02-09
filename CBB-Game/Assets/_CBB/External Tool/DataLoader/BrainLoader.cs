using ArtificialIntelligence.Utility;
using CBB.DataManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrainLoader : MonoBehaviour
{
    public class Memento
    {
        private readonly string m_brainName;
        private string m_agent_ID;
        public string Agent_ID { get => m_agent_ID; private set => m_agent_ID = value; }
        public Memento(string brainName, string agent_ID)
        {
            this.m_brainName = brainName;
            this.m_agent_ID = agent_ID;
        }
    }
    #region Fields

    public string m_agent_ID;

    private List<ActionState> actionStates = new();
    private List<Sensor> sensors = new();

    private Brain brain;
    private AgentBrain agentBrain;
    #endregion
    [HideInInspector]
    public string m_brainName;
    [SerializeField]
    private bool showLogs = false;

    public string BrainName
    {
        get => m_brainName;
        set
        {
            m_brainName = value;
            if (showLogs) Debug.Log($"[BRAIN LOADER] Brain name updated: {m_brainName}");
        }
    }
    private void Awake()
    {
        agentBrain = GetComponent<AgentBrain>();
        DataLoader.BrainUpdated += ReadBrain;
    }
    private void Start()
    {
        // Check if the agent has a brain associated
        var bindingData = BindingManager.AgentIDBrainID.data;
        if (!bindingData.ContainsKey(m_agent_ID))
        {
            Debug.LogWarning("Agent has no associated brain");
            return;
        }
        var brain_ID = bindingData[m_agent_ID];
        brain = DataLoader.GetBrainByID(brain_ID);
        InitializeAgentWithBrain(brain);
        agentBrain.TryStartNewAction();
    }
    private void OnDestroy()
    {
        DataLoader.BrainUpdated -= ReadBrain;
    }
    
    /// <summary>
    /// Update the brain data with the current configuration
    /// </summary>
    /// <param name="brain_ID"></param>
    public void ReadBrain(string brain_ID)
    {
        // TODO:
        var bindingData = BindingManager.AgentIDBrainID.data;
        if (!bindingData.ContainsKey(m_agent_ID))
        {
            Debug.LogWarning("Agent has no associated brain");
            return;
        }
        if (bindingData[m_agent_ID] != brain_ID)
        {
            Debug.LogWarning("This is not the brain associated with this agent");
            return;
        }
        // All checked, update the brain
        StartCoroutine(UpdateAgentBehaviourWithBrain(brain_ID));
    }
    // NOTE: In order to not break the agent (stall, infinite loop, etc) is necessary
    // to pause the agent, update the brain and then resume the agent on several steps (frames)
    private IEnumerator UpdateAgentBehaviourWithBrain(string brain_ID)
    {
        var memento = GetMemento();
        agentBrain.Pause();
        yield return null;

        var brain = DataLoader.GetBrainByID(brain_ID);
        InitializeAgentWithBrain(brain);
        BindingManager.UpdateAgentIDBrainIDBinding(memento, m_agent_ID, m_brainName);
        yield return null;

        agentBrain.Resume();
        if (showLogs) Debug.Log($"[BRAIN LOADER] Agent updated with brain: {brain.brain_Name}");
    }

    public void InitializeAgentWithBrain(Brain brain)
    {
        // Find monobehaviours refs
        GetBehaviourComponents();

        this.brain = brain;
        var szedAction = brain.serializedActions;

        foreach (var action in actionStates)
        {
            if (!szedAction.Exists(x => x.ClassType == action.GetType()))
            {
                Destroy(action);
                break;
            }
        }
        for (int i = 0; i < szedAction.Count; i++)
        {
            var act = actionStates.Find(x => x.GetType() == szedAction[i].ClassType);
            if (act == null)
            {
                act = gameObject.AddComponent(szedAction[i].ClassType) as ActionState;
            }
            act.SetParams(szedAction[i]);
        }

        var szedSensor = brain.serializedSensors;
        foreach (var sensor in sensors)
        {
            if (!szedSensor.Exists(x => x.ClassType == sensor.GetType()))
            {
                Destroy(sensor);
                break;
            }
        }
        for (int i = 0; i < szedSensor.Count; i++)
        {
            var sens = sensors.Find(x => x.GetType() == szedSensor[i].ClassType);
            if (sens == null)
            {
                sens = gameObject.AddComponent(szedSensor[i].ClassType) as Sensor;
            }
            sens.SetParams(szedSensor[i]);
        }

        agentBrain.ReloadBehaviours();
    }

    public void GetBehaviourComponents()
    {
        GetAssignedActions();
        GetAssignedSensors();
    }

    private void GetAssignedActions()
    {
        actionStates.Clear();
        actionStates.AddRange(gameObject.GetComponentsOnHierarchy<ActionState>());
    }

    private void GetAssignedSensors()
    {
        sensors.Clear();
        sensors.AddRange(gameObject.GetComponentsOnHierarchy<Sensor>());
    }

    public Memento GetMemento()
    {
        return new Memento(m_brainName, m_agent_ID);
    }
}