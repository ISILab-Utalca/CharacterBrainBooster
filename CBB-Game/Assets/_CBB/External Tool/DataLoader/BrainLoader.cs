using ArtificialIntelligence.Utility;
using Generic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrainLoader : MonoBehaviour
{
    #region Fields
    [Tooltip("If activated, it bypasses the brain system and uses the default configuration.")]
    public bool _default = false;
    [Tooltip("Generate a brain file if it doesn't exist.")]
    public bool createBrain = false;

    public string agent_ID;

    private List<ActionState> actionStates = new();
    private List<Sensor> sensors = new();
    private Brain brain;
    private AgentBrain agentBrain;
    [SerializeField]
    private bool showLogs = false; 
    #endregion

    private void Awake()
    {
        agentBrain = GetComponent<AgentBrain>();
        DataLoader.BrainUpdated += ReadBrain;
    }
    private void Start()
    {
        // If default is activated, bypass the brain system
        if (_default) return;
        
        // Check if the agent has a brain associated
        var pair = DataLoader.GetPairByAgentID(agent_ID);
        if (pair == null)
        {
            if (createBrain)
            {
                var b = CreateBrainFile();
                //TODO: BRAIN_ID must be different from agent_ID
                DataLoader.SaveBrain(this.agent_ID, b);
                DataLoader.AddPair(new PairBrainData.PairBrain() { agent_ID = agent_ID, brain_ID = b.brain_ID });
                InitAgent(b);
                agentBrain.TryStartNewAction(null);
            }
            return;
        }

        // Check if the brain associated with the agent exists
        var brain = DataLoader.GetBrainByID(pair.brain_ID);
        if (brain == null)
        {
            if (createBrain)
            {
                //TODO: BRAIN_ID must be different from agent_ID
                var b = CreateBrainFile();
                DataLoader.SaveBrain(this.agent_ID, b);
                DataLoader.ReplacePair(new PairBrainData.PairBrain() { agent_ID = agent_ID, brain_ID = b.brain_ID });
                InitAgent(b);
                agentBrain.TryStartNewAction(null);
            }
            return;
        }
        
        // All checked, update the brain and start the agent behaviour
        InitAgent(brain);
        agentBrain.TryStartNewAction(null);
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
        // Check if the updated brain ID matches the brain associated with the agent
        if (brain_ID == null) return;
        var pair = DataLoader.GetPairByAgentID(this.agent_ID);
        if (pair == null) return;
        if (brain_ID != pair.brain_ID) return;

        // All checked, update the brain
        StartCoroutine(PauseReadUpdate(brain_ID));
    }
    // NOTE: In order to not break the agent (stall, infinite loop, etc) is necessary
    // to pause the agent, update the brain and then resume the agent on several steps (frames)
    private IEnumerator PauseReadUpdate(string brain_ID)
    {
        this.agent_ID = brain_ID;
        agentBrain.Pause();
        yield return null;

        var brain = DataLoader.GetBrainByID(brain_ID);
        InitAgent(brain);
        yield return null;

        agentBrain.Resume();
        if (showLogs) Debug.Log($"[BRAIN LOADER] Agent updated with brain: {brain_ID}");
    }

    public void OnApplicationQuit()
    {
        DataLoader.SaveBrain(this.agent_ID, brain); // parche (!!!) quitar mas adelante
    }

    /// <summary>
    /// create a brain file with the current configuration
    /// </summary>
    public Brain CreateBrainFile()
    {
        FindBHsReferences();
        //TODO: BRAIN_ID must be different from agent_ID
        brain = new Brain
        {
            brain_ID = agent_ID,
            serializedActions = new List<DataGeneric>(),
            serializedSensors = new List<DataGeneric>()
        };

        for (int i = 0; i < actionStates.Count; i++)
        {
            brain.serializedActions.Add(actionStates[i].GetGeneric());
        }

        for (int i = 0; i < sensors.Count; i++)
        {
            brain.serializedSensors.Add(sensors[i].GetGeneric());
        }

        return brain;
    }


    /// <summary>
    /// find monobehaviours related to the brain and store them in lists
    /// enable the brain to be initialized with the brain data
    /// </summary>
    public void FindBHsReferences()
    {
        // Get all actions in this game object and its children
        actionStates.Clear();
        actionStates.AddRange(gameObject.GetComponentsOnHierarchy<ActionState>());

        // Get all sensors in this game object and its children
        sensors.Clear();
        sensors.AddRange(gameObject.GetComponentsOnHierarchy<Sensor>());
    }

    /// <summary>
    /// Initialize the brain with the brain data
    /// </summary>
    /// <param name="brain"></param>
    public void InitAgent(Brain brain)
    {
        // Find monobehaviours refs
        FindBHsReferences();

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
}