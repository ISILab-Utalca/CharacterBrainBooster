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
    [Tooltip("Generate a brain file if it doesn't exist.")]
    public bool CrerateBrain = false;

    public string agent_ID;

    private List<ActionState> actionStates = new List<ActionState>();
    private List<Sensor> sensors = new List<Sensor>();

    private void Start()
    {
        // Find monobehaviours refs
        FindBHsReferences();

        // Load brain data
        DataLoader.Init();
        var brain = DataLoader.GetBrainByAgentID(agent_ID);

        if (brain != null)
        {
            Init(brain);
        }
        else if (CrerateBrain)
        {
            CreateBrainFile();
        }
    }

    private void Reset()
    {
        agent_ID = this.gameObject.name;
    }

    /// <summary>
    /// create a brain file with the current configuration
    /// </summary>
    public void CreateBrainFile()
    {
        var brain = new Brain();
        brain.brain_ID = agent_ID;
        brain.serializedActions = new List<DataGeneric>();
        brain.serializedSensors = new List<DataGeneric>();

        for (int i = 0; i < actionStates.Count; i++)
        {
            brain.serializedActions.Add(actionStates[i].GetGeneric());
        }

        for (int i = 0; i < sensors.Count; i++)
        {
            brain.serializedSensors.Add(sensors[i].GetGeneric());
        }

        DataLoader.SaveBrain(brain);
    }

    /// <summary>
    /// find monobehaviours related to the brain and store them in lists
    /// enable the brain to be initialized with the brain data
    /// </summary>
    public void FindBHsReferences()
    {
        // Get all actions in this game object and its children
        actionStates = GetComponents<ActionState>().ToList();
        actionStates.AddRange(GetComponentsInChildren<ActionState>());

        // Get all sensors in this game object and its children
        sensors = GetComponents<Sensor>().ToList();
        sensors.AddRange(GetComponentsInChildren<Sensor>());
    }

    /// <summary>
    /// Initialize the brain with the brain data
    /// </summary>
    /// <param name="brain"></param>
    public void Init(Brain brain)
    {
        var szedAction = brain.serializedActions;
        for (int i = 0; i < szedAction.Count; i++)
        {
            var act = actionStates.Find(x => x.GetType() == szedAction[i].GetType());
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
            var sens = sensors.Find(x => x.GetType() == szedSensor[i].GetType());
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
public class Brain
{
    public string brain_ID;
    [SerializeField,SerializeReference]
    public List<DataGeneric> serializedActions;
    [SerializeField,SerializeReference]
    public List<DataGeneric> serializedSensors;
}



