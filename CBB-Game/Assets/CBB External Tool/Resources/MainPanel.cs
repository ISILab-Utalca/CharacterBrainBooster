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
    [SerializeField]
    private GameObject utilityMain;

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
        this.openBrain.clicked += () => { new NotImplementedException(); }; // (!)

    }


    private void CreateBrain()
    {
        Globals.Current = new AgentData(_agentType);
        this.gameObject.SetActive(false);
        utilityMain.SetActive(true);
    }
}

public static class Globals
{
    public static AgentData Current;

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

