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
    private Button SaveButton;

    private Button conditionsTab;
    private Button actionsTab;
    private Button settingsTab;

    private VisualElement actionsContent;
    private Button addAction;

    private VisualElement considerationsContent;
    private Button addConsideration;

    private SettingsPanel settingsPanel;

    private AgentBrainData _current;

    private void Awake()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        _current = Globals.Current;

        // NameLabel
        this.nameLabel = root.Q<Label>("NameLabel");
        var type = _current.baseData.agentType;
        this.nameLabel.text = (type.GetCustomAttributes(typeof(UtilityAgentAttribute), false)[0] as UtilityAgentAttribute).Name;
        
        // Considerations Content
        this.considerationsContent = root.Q<VisualElement>("ConsiderationsContent");

        // Actions Content
        this.actionsContent = root.Q<VisualElement>("ActionsContent");

        // Settings panel
        this.settingsPanel = root.Q<SettingsPanel>("SettingsPanel");

        // SaveButton
        this.SaveButton = root.Q<Button>("TestButton");
        SaveButton.clicked += () => { SaveTest("Test");  }; // (!)

        // AddConsideration
        this.addConsideration = root.Q<Button>("AddConsideration");
        this.addConsideration.clicked += () => { AddConsideration(); };

        // Actions Tab
        this.actionsTab = root.Q<Button>("ActionsTab");
        this.actionsTab.clicked += () => {
            ChangeTab(actionsContent);

        };

        // Conditions Tab
        this.conditionsTab = root.Q<Button>("ConditionsTab");
        this.conditionsTab.clicked += () => {
            ChangeTab(considerationsContent);
            UpdateConsiderations();
        };

        // Settings Tab
        this.settingsTab = root.Q<Button>("SettingsTab");
        this.settingsTab.clicked += () => {
            ChangeTab(settingsPanel);
        };

        // Init
        ChangeTab(considerationsContent);
        UpdateConsiderations();

    }

    private void ChangeTab(VisualElement tab)
    {
        actionsContent.style.display = (actionsContent == tab) ? DisplayStyle.Flex : DisplayStyle.None;
        considerationsContent.style.display = (considerationsContent == tab) ? DisplayStyle.Flex : DisplayStyle.None;
        settingsPanel.style.display = (settingsPanel == tab) ? DisplayStyle.Flex : DisplayStyle.None;
    }


    private void UpdateConsiderations()
    {
        var considerations = _current.considerations;
        for (int i = 0; i < considerations.Count; i++)
        {
            var cons = considerations[i];
            this.considerationsContent.Add(new UtilityPanel(_current, cons, () => { 
                this.considerationsContent.Clear(); 
                UpdateConsiderations();
                this.considerationsContent.Add(this.addConsideration);
            })); 
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
            true,
            new List<Variable>(),
            new Normalize(),
            new Linear()
            );
        _current.considerations.Add(consideration);
        this.considerationsContent.Clear();
        UpdateConsiderations();
        this.considerationsContent.Add(this.addConsideration);
    }

    public void SaveTest(string fileName)
    {
        var path = Application.persistentDataPath;
        Utility.JSONDataManager.SaveData<AgentBrainData>(path, fileName, _current);
        Debug.LogFormat("Saved on: <a href=\"{0}\">{0}</a>", "file://" + path + "/" + fileName +".json");
        Application.OpenURL("file://" + path + "/" + fileName + ".json");
        //Debug.LogFormat("Haz clic aquí para abrir el archivo de código: <a href=\"{0}\">{0}</a>", "file:///C:/ruta/al/archivo.cs");
        //Debug.Log("Saved on: " + path);
    }
}
