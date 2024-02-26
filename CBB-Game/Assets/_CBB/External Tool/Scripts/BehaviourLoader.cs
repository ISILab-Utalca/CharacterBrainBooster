using ArtificialIntelligence.Utility;
using CBB.Comunication;
using CBB.DataManagement;
using CBB.Lib;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class BehaviourLoader : MonoBehaviour
{
    /// <summary>
    /// Holds the previous state of the BrainLoader
    /// https://www.dofactory.com/net/memento-design-pattern
    /// </summary>
    public class Memento
    {
        private readonly string m_brainName;
        private string m_subGroupName;
        public string SubGroupName { get => m_subGroupName; private set => m_subGroupName = value; }
        public Memento(string brainName, string agent_ID)
        {
            this.m_brainName = brainName;
            this.m_subGroupName = agent_ID;
        }
    }
    [System.Serializable]
    public class AssociatedAgentBehaviour
    {
        public string m_agentInstanceID;
        public string m_agentType;
        public string m_agentTypeSubgroup;
        public string m_brainName;
        public string m_brainID;
        public AssociatedAgentBehaviour(string agentInstanceID, string agentType, string subGroup, string brainName, string brainID)
        {
            m_brainID = brainID;
            m_brainName = brainName;
            m_agentType = agentType;
            m_agentTypeSubgroup = subGroup;
            m_agentInstanceID = agentInstanceID;
        }
    }

    #region Fields
    public string m_agentType = "Default";
    public string m_agentTypeSubgroup;
    public string m_brainName;

    private List<ActionState> actionStates = new();
    private List<Sensor> sensors = new();
    private Brain m_brain;
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
        Server.OnNewClientConnected += SendBindingData;
    }
    public void UpdateBehaviour(Brain brain)
    {
        var bindingData = BindingManager.AgentIDBrainID.data;
        if (!bindingData.ContainsKey(m_agentType))
        {
            Debug.LogWarning("Agent has no associated brain");
            return;
        }
        if (bindingData[m_agentType] != brain.id)
        {
            Debug.LogWarning("This is not the brain associated with this agent");
            return;
        }
        StartCoroutine(ResetAgentBehaviour(brain));
    }
    private void SendBindingData(TcpClient client)
    {
        var association = new AgentBrainAssociation("agentType", GetMemento().SubGroupName, m_brainName, gameObject.name, gameObject.GetInstanceID());
        var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All, NullValueHandling = NullValueHandling.Include };
        var serializedMessage = JsonConvert.SerializeObject(association, settings);
        Server.SendMessageToClient(client, serializedMessage);
    }


    private void Start()
    {
        if (AgentHasBrain())
        {
            SetupAgentBehaviour(m_brain);
            agentBrain.TryStartNewAction();
        }
    }
    private bool AgentHasBrain()
    {
        var bindingData = BindingManager.AgentIDBrainID.data;
        if (!bindingData.ContainsKey(m_agentType)) return false;

        var brain_ID = bindingData[m_agentType];
        m_brain = BrainDataLoader.GetBrainByID(brain_ID);
        return m_brain != null;
    }
    public void SetupAgentBehaviour(Brain brain)
    {
        GetBehaviourComponents();

        this.m_brain = brain;
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
    private void OnDestroy()
    {
        BrainDataLoader.BrainUpdated -= UpdateBehaviour;
        Server.OnNewClientConnected -= SendBindingData;
    }

    private IEnumerator ResetAgentBehaviour(Brain brain)
    {
        // NOTE: In order to not break the agent (stall, infinite loop, etc) is necessary
        // to pause the agent, update the brain and then resume the agent on several steps (frames)
        var memento = GetMemento();
        agentBrain.Pause();
        yield return null;

        SetupAgentBehaviour(brain);
        BindingManager.UpdateAgentIDBrainIDBinding(memento, m_agentType, m_brainName);
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
        return new Memento(m_brainName, m_agentType);
    }
}