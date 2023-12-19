using ArtificialIntelligence.Utility;
using Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class BrainLoader : MonoBehaviour
{

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
    private void Awake()
    {
        agentBrain = GetComponent<AgentBrain>();
    }
    private void Start()
    {
        // If default is activated, bypass the brain system
        if(_default)
            return;

        // Get the pair data by the agent ID
        var pair = DataLoader.GetPairByAgentID(agent_ID);
        if(pair == null)
        {
            if (createBrain)
            {
                var b = CreateBrainFile();
                DataLoader.SaveBrain(this.agent_ID, b);
                DataLoader.AddPair(new PairBrainData.PairBrain() { agent_ID = agent_ID, brain_ID = b.brain_ID });
            }
            return;
        }

        // Get the brain data by the brain ID
        var brain = DataLoader.GetBrainByID(pair.brain_ID);
        if(brain == null)
        {
            if(createBrain)
            {
                var b = CreateBrainFile();
                DataLoader.SaveBrain(this.agent_ID, b);
                DataLoader.ReplacePair(new PairBrainData.PairBrain() { agent_ID = agent_ID, brain_ID = b.brain_ID });
            }
            return;
        }

        // Initialize the brain with the brain data
        InitAgent(brain);
    }
    /// <summary>
    /// Update the brain data with the current configuration
    /// </summary>
    /// <param name="agent_ID"></param>
    public void ReadBrain(string agent_ID)
    {
        StartCoroutine(PauseReadUpdate(agent_ID));
    }
    // NOTE: In order to not break the agent (stall, infinite loop, etc) is necessary
    // to pause the agent, update the brain and then resume the agent on several steps (frames)
    private IEnumerator PauseReadUpdate(string agent_ID)
    {
        this.agent_ID = agent_ID;
        agentBrain.Pause();
        yield return null;
        Start();
        yield return null;
        agentBrain.Resume();
        if(showLogs) Debug.Log($"[BRAIN LOADER] Brain updated for agent {agent_ID}");
    }

    public void OnApplicationQuit()
    {
        DataLoader.SaveBrain(this.agent_ID, brain); // parche (!!!) quitar mas adelante
    }

#if UNITY_EDITOR
    [ContextMenu("Debug serialized brains")]
    public void DebugBrains()
    {
        DataLoader.SendBrains(null);
    }
#endif
    /// <summary>
    /// create a brain file with the current configuration
    /// </summary>
    public Brain CreateBrainFile()
    {
        FindBHsReferences();

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
        actionStates.AddRange(GetComponentsInChildren<ActionState>());

        // Get all sensors in this game object and its children
        sensors.AddRange(GetComponentsInChildren<Sensor>());
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
        for (int i = 0; i < szedAction.Count; i++)
        {
            var act = actionStates.Find(x => x.GetType() == szedAction[i].ClassType);
            if (act != null)
            {
                act.SetParams(szedAction[i]);
                actionStates.Remove(act);
            }
            else
            {
                act = this.gameObject.AddComponent(szedAction[i].ClassType) as ActionState;
                act.SetParams(szedAction[i]);
            }
        }

        var szedSensor = brain.serializedSensors;
        for (int i = 0; i < szedSensor.Count; i++)
        {
            var sens = sensors.Find(x => x.GetType() == szedSensor[i].ClassType);
            if (sens != null)
            {
                sens.SetParams(szedSensor[i]);
                sensors.Remove(sens);
            }
            else
            {
                sens = this.gameObject.AddComponent(szedSensor[i].ClassType) as Sensor;
                sens.SetParams(szedSensor[i]);
            }
        }
    }
}

#if UNITY_EDITOR
[Editor(typeof(BrainLoader))]
public class BrainLoaderEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var BL = (BrainLoader)target;
        if (BL.agent_ID == "")
        {
            GUI.Box(new Rect(0, 0, 100, 100), "Agrege una ID para el agente para que no tenga errores.");
        }
    }
}
#endif

/// <summary>
/// this class is used to store the generic brain data
/// </summary>
[System.Serializable]
public class Brain : IDataItem
{
    public string brain_ID;
    [SerializeField,SerializeReference]
    public List<DataGeneric> serializedActions;
    [SerializeField,SerializeReference]
    public List<DataGeneric> serializedSensors;

    public string GetItemName()
    {
        return brain_ID;
    }
}
public interface IDataItem
{
    string GetItemName();
}



