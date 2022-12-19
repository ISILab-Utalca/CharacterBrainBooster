using CBB.Api;
using CBB.Lib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class MainPanel : MonoBehaviour
{
    // Panel references
    [SerializeField]
    private GameObject _utilityMain;
    [SerializeField]
    private GameObject _openBrainPanel;

    private DropdownField agentDropdown;
    private Button createBrain;
    private Button openBrain;

    private Type _agentType;

    private void Awake()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        
        // AgentDropdown
        this.agentDropdown = root.Q<DropdownField>("AgentDropdown");
        var agentTypes = UtilitySystem.CollectAgentTypes();
        this.agentDropdown.choices = agentTypes.Select((t) => {
            var att = t.GetCustomAttributes(typeof(UtilityAgentAttribute),false)[0] as UtilityAgentAttribute;
            return att.Name;
            }).ToList();
        this.agentDropdown.index = 0;
        _agentType = agentTypes[0];
        this.agentDropdown.RegisterCallback<ChangeEvent<string>>(e => {
            var index = this.agentDropdown.index;
            _agentType = agentTypes[index];
        });

        // CreateBrain
        this.createBrain = root.Q<Button>("CreateBrain");
        this.createBrain.clicked += () => { CreateBrain(); };

        // OpenBrain
        this.openBrain = root.Q<Button>("OpenBrain");
        this.openBrain.clicked += () => { OpenBrain(); }; // (!)

    }


    private void CreateBrain()
    {
        Globals.Current = new AgentBrainData(_agentType);
        this.gameObject.SetActive(false);
        _utilityMain.SetActive(true);
    }

    private void OpenBrain()
    {
        this.gameObject.SetActive(false);
        _openBrainPanel.SetActive(true);
    }
}

public static class Globals
{
    public static AgentBrainData Current;

    public static List<Variable> globalVariables = new List<Variable>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void OnBeforeSceneLoadRuntimeMethod()
    {
        Debug.Log("Before scene loaded");
        var agentTypes = UtilitySystem.CollectAgentTypes();
        var allVariables = agentTypes.Select( t => UtilitySystem.CollectVariables(t)).ToList();

        allVariables.ForEach(vs => globalVariables.AddRange(vs));
    }
}

