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
        this.agentDropdown.choices = Globals.AgentClasses.Select((t) => {
            var att = t.GetCustomAttributes(typeof(UtilityAgentAttribute),false)[0] as UtilityAgentAttribute;
            return att.Name;
            }).ToList();
        this.agentDropdown.index = 0;
        _agentType = Globals.AgentClasses[0];
        this.agentDropdown.RegisterCallback<ChangeEvent<string>>(e => {
            var index = this.agentDropdown.index;
            _agentType = Globals.AgentClasses[index];
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

    private static Type[] agentClasses;
    public static Type[] AgentClasses 
    {
        get 
        {
            if(agentClasses == null)
                agentClasses = AgentData.Collect();
            return agentClasses;
        } 
    }

    /*
    private static UtilityEvaluator[] evaluators;
    public static UtilityEvaluator[] Evaluators
    {
        get
        {
            if (evaluators == null)
                evaluators = UtilityEvaluator.GetEvaluators().ToArray();
            return evaluators;
        }
    }
    */
}

