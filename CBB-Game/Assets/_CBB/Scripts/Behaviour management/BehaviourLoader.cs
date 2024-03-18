using ArtificialIntelligence.Utility;
using CBB.DataManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourLoader : MonoBehaviour
{

    #region Fields
    public string m_agentType = "Default";
    public string m_agentTypeSubgroup;
    public string m_brainName;

    private Brain m_brain;
    private List<ActionState> m_actionStates = new();
    private List<Sensor> m_sensors = new();
    private AgentBrain agentBrain;
    #endregion
    public string BrainName
    {
        get => m_brainName;
        set
        {
            m_brainName = value;
        }
    }

    private void Awake()
    {
        agentBrain = GetComponent<AgentBrain>();
        BrainDataLoader.BrainUpdated += UpdateBehaviour;
    }
    public void UpdateBehaviour(Brain brain)
    {
        if (GetAssociatedBrain().id == brain.id)
        {
            Debug.LogWarning($"It's a match: {gameObject.name} <> {brain.name} <> {brain.id}");
            StartCoroutine(ResetAgentBehaviour(brain));
        }
    }
    private void Start()
    {
        var brain = GetAssociatedBrain();
        if (brain == null)
        {
            Debug.LogWarning("No brain associated with this agent");
            return;
        }
        SetupAgentBehaviour(brain);
        agentBrain.TryStartNewAction();
    }
    private Brain GetAssociatedBrain()
    {
        var brainMaps = BrainMapsManager.GetAllBrainMaps();
        if (brainMaps == null) return null;
        var subgroup = brainMaps.Find(x => x.agentType == m_agentType).SubgroupsBrains.Find(x => x.subgroupName == m_agentTypeSubgroup);
        if (subgroup == null) return null;
        var brain_ID = subgroup.brainID;
        return BrainDataLoader.GetBrainByID(brain_ID);
    }
    public void SetupAgentBehaviour(Brain brain)
    {
        GetBehaviourComponents();

        this.m_brain = brain;
        var szedAction = brain.serializedActions;

        foreach (var action in m_actionStates)
        {
            if (!szedAction.Exists(x => x.ClassType == action.GetType()))
            {
                Destroy(action);
                //break;
            }
        }
        for (int i = 0; i < szedAction.Count; i++)
        {
            var act = m_actionStates.Find(x => x.GetType() == szedAction[i].ClassType);
            if (act == null)
            {
                act = gameObject.AddComponent(szedAction[i].ClassType) as ActionState;
            }
            act.SetParams(szedAction[i]);
        }

        var szedSensor = brain.serializedSensors;
        foreach (var sensor in m_sensors)
        {
            if (!szedSensor.Exists(x => x.ClassType == sensor.GetType()))
            {
                Destroy(sensor);
                //break;
            }
        }
        for (int i = 0; i < szedSensor.Count; i++)
        {
            var sens = m_sensors.Find(x => x.GetType() == szedSensor[i].ClassType);
            if (sens == null)
            {
                sens = gameObject.AddComponent(szedSensor[i].ClassType) as Sensor;
            }
            sens.SetParams(szedSensor[i]);
        }

        agentBrain.ReloadBehaviours();
    }
    private void OnDestroy()
    {
        BrainDataLoader.BrainUpdated -= UpdateBehaviour;
    }
    private IEnumerator ResetAgentBehaviour(Brain brain)
    {
        // NOTE: In order to not break the agent (stall, infinite loop, etc) is necessary
        // to pause the agent, update the brain and then resume the agent on several steps (frames)
        agentBrain.Pause();
        while(agentBrain.IsRunningAction())
        {
            yield return null;
        }
        SetupAgentBehaviour(brain);
        yield return null;

        agentBrain.Resume();
    }
    public void GetBehaviourComponents()
    {
        GetAssignedActions();
        GetAssignedSensors();
    }
    private void GetAssignedActions()
    {
        m_actionStates.Clear();
        m_actionStates.AddRange(gameObject.GetComponentsOnHierarchy<ActionState>());
    }
    private void GetAssignedSensors()
    {
        m_sensors.Clear();
        m_sensors.AddRange(gameObject.GetComponentsOnHierarchy<Sensor>());
    }
}