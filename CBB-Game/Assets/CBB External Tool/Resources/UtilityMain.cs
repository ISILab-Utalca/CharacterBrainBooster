using CBB.Api;
using CBB.Lib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class UtilityMain : MonoBehaviour
{
    private Label nameLabel;
    private VisualElement content;

    private Button Test;
    private Button addConsideration;

    private AgentData _current;

    private void Awake()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        _current = Globals.Current;

        // NameLabel
        this.nameLabel = root.Q<Label>("NameLabel");
        this.nameLabel.text = (_current.type.GetCustomAttributes(typeof(UtilityAgentAttribute), false)[0] as UtilityAgentAttribute).Name;
        
        // Content panel
        this.content = root.Q<VisualElement>("Content");
        UpdateConsiderations();

        // SaveButton
        this.Test = root.Q<Button>("TestButton");
        Test.clicked += () => { SaveTest("Test");  }; // (!)

        // AddConsideration
        this.addConsideration = root.Q<Button>("AddConsideration");
        this.addConsideration.clicked += () => { AddConsideration(); };

    }

    private void UpdateConsiderations()
    {
        var considerations = _current.considerations;
        for (int i = 0; i < considerations.Count; i++)
        {
            var cons = considerations[i];
            this.content.Add(new UtilityPanel(_current, cons, () => { 
                this.content.Clear(); 
                UpdateConsiderations();
                this.content.Add(this.addConsideration);
            })); // (?) ojo recursivo ?
        }
    }

    private void AddConsideration()
    {
        var newName = "";
        var iterator = 0;
        do{
            newName = "New consideration " + iterator;
            iterator++;
        } while (_current.considerations.Any( c => c.name == newName));

        var consideration = new Consideration(
            newName,
            new List<Variable>(),
            new Normalize(),
            new Linear()
            );
        _current.considerations.Add(consideration);
        this.content.Clear();
        UpdateConsiderations();
        this.content.Add(this.addConsideration);
    }

    public void SaveTest(string fileName)
    {
        var path = Application.persistentDataPath + "/" + fileName + ".json";
        Utility.JSONDataManager.SaveData<AgentData>(path, _current);
        Debug.Log("Saved on: " + path);
    }
}
