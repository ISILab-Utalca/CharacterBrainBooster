using CBB.ExternalTool;
using CBB.Lib;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

public class ConsiderationEditorController : MonoBehaviour
{
    DropdownField curveDropdown;
    //VisualElement root;
    TextField considerationName;
    VisualElement curveParametersContainer;
    Chart chart;
    Curve selectedCurve;
    Button saveButton;
    Button sendButton;
    Toggle normalizeInput;
    FloatField minValue;
    FloatField maxValue;
    ExternalMonitor externalMonitor;
    JsonSerializerSettings settings = new()
    {
        MissingMemberHandling = MissingMemberHandling.Error,
        PreserveReferencesHandling = PreserveReferencesHandling.Objects,
        TypeNameHandling = TypeNameHandling.Auto,
        Formatting = Formatting.Indented
    };
    //private void Awake()
    //{
    //    externalMonitor = FindObjectOfType<ExternalMonitor>();
    //    root = GetComponent<UIDocument>().rootVisualElement;

    //    considerationName = root.Q<TextField>("consideration-name");
        
    //    curveDropdown = root.Q<DropdownField>("curve-type-dropdown");
        
    //    curveParametersContainer = root.Q<VisualElement>("curve-parameters-container");
        
    //    saveButton = root.Q<Button>("save-button");
    //    sendButton = root.Q<Button>("send-button");
        
    //    chart = root.Q<Chart>();
        
    //    normalizeInput = root.Q<Toggle>("normalize-input");
        
    //    minValue = root.Q<FloatField>("min-value");
    //    maxValue = root.Q<FloatField>("max-value");
        
    //    // Populate the dropdown with the curve types
    //    var curves = Curve.GetCurves();
    //    var curveNames = new List<string>();
    //    foreach (var curve in curves)
    //    {
    //        curveNames.Add(curve.GetType().Name);
    //    }
    //    curveDropdown.choices = curveNames;
    //}
    private void Subscribe()
    {
        saveButton.clicked += SaveCurveData;
        sendButton.clicked += SendConfiguration;
        curveDropdown.RegisterValueChangedCallback(OnCurveTypeChanged);
        normalizeInput.RegisterValueChangedCallback(DisplayRange);
    }


    private void Unsubscribe()
    {
        saveButton.clicked -= SaveCurveData;
        sendButton.clicked -= SendConfiguration;
        curveDropdown.UnregisterValueChangedCallback(OnCurveTypeChanged);
        normalizeInput.UnregisterValueChangedCallback(DisplayRange);
    }
    /// <summary>
    /// Hide or show the min/max float fields based on the normalize input toggle
    /// </summary>
    /// <param name="evt"></param>
    private void DisplayRange(ChangeEvent<bool> evt)
    {
        minValue.style.display = evt.newValue ? DisplayStyle.Flex : DisplayStyle.None;
        maxValue.style.display = minValue.style.display;
    }
    
    /// <summary>
    /// Iterate through the curve parameters container and save the values
    /// on a json file
    private void SaveCurveData()
    {
        ConsiderationConfiguration cc = new()
        {
            name = considerationName.text,
            curve = selectedCurve,
            normalizeInput = normalizeInput.value,
            minValue = minValue.value,
            maxValue = maxValue.value
        };
        
        var jsonObject = JsonConvert.SerializeObject(cc,settings);
        // Save the json object to a file
        string path = Application.dataPath + "/CBB External Tool/Resources/ConsiderationConfigurations/" + considerationName.text + ".json";
        Debug.Log(path);
        // Create the directory if it doesn't exist
        System.IO.Directory.CreateDirectory(Application.dataPath + "/CBB External Tool/Resources/ConsiderationConfigurations/");
        System.IO.File.WriteAllText(path, jsonObject.ToString());
    }
    private void OnCurveTypeChanged(ChangeEvent<string> evt)
    {
        // Show curve parameters dynamically based on curve type
        // Get the curve type from the dropdown
        selectedCurve = Curve.GetCurves().Find(x => x.GetType().Name == evt.newValue);
        // Get the curve parameters from the curve type
        var curveParameters = selectedCurve.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
        // Get the curve parameters container
        // Clear the container
        curveParametersContainer.Clear();
        // Add the curve parameters to the container

        foreach (var curveParameter in curveParameters)
        {
            //First, check if the parameter's type is boolean
            if (curveParameter.FieldType == typeof(bool))
            {
                Toggle toggle = new()
                {
                    label = curveParameter.Name,
                    value = (bool)curveParameter.GetValue(selectedCurve)
                };
                toggle.RegisterValueChangedCallback(UpdateChart);
                toggle.labelElement.style.fontSize = 20;
                toggle.style.color = Color.white;
                curveParametersContainer.Add(toggle);
                continue;
            }
            FloatField floatField = new()
            {
                label = curveParameter.Name,
                value = (float)curveParameter.GetValue(selectedCurve)
            };
            floatField.RegisterValueChangedCallback(UpdateChart);
            //Set the style
            floatField.style.color = Color.white;
            floatField.labelElement.style.fontSize = 20;

            curveParametersContainer.Add(floatField);
        }
        chart.SetCurve(selectedCurve);
    }
    /// <summary>
    /// Reflect the changes made to the curve's float parameters
    /// on the graphic chart
    /// </summary>
    private void UpdateChart(ChangeEvent<float> evt)
    {
        // Get the name of the field using the label of the field info
        string fieldname = ((FloatField)evt.currentTarget).label;
        selectedCurve.GetType().GetField(fieldname).SetValue(selectedCurve, evt.newValue);
        chart.SetCurve(selectedCurve);
    }
    /// <summary>
    /// Reflect the changes made to the curve's bool parameters
    /// on the graphic chart
    /// </summary>
    private void UpdateChart(ChangeEvent<bool> evt)
    {
        // Get the name of the field using the label of the field info
        string fieldname = ((Toggle)evt.currentTarget).label;
        selectedCurve.GetType().GetField(fieldname).SetValue(selectedCurve, evt.newValue);
        chart.SetCurve(selectedCurve);
    }

    private void SendConfiguration()
    {
        ConsiderationConfiguration cc = new()
        {
            name = considerationName.text,
            curve = selectedCurve,
            normalizeInput = normalizeInput.value,
            minValue = minValue.value,
            maxValue = maxValue.value
        };

        var jsonObject = JsonConvert.SerializeObject(cc, settings);
        // No need to handle a reference on the inspector
        
        externalMonitor.SendConfiguration(jsonObject);
    }
    public void ShowConsideration(ConsiderationConfiguration consideration)
    {
        chart.SetCurve(consideration.curve);
        curveDropdown.value = consideration.curve.GetType().Name;
        considerationName.value = consideration.name;
        minValue.value = consideration.minValue;
        maxValue.value = consideration.maxValue;
        normalizeInput.value = consideration.normalizeInput;
    }
    public void SetConsiderationEditor(ConsiderationEditor considerationEditor)
    {
        considerationName = considerationEditor.Q<TextField>("consideration-name");

        curveDropdown = considerationEditor.Q<DropdownField>("curve-type-dropdown");

        curveParametersContainer = considerationEditor.Q<VisualElement>("curve-parameters-container");

        saveButton = considerationEditor.Q<Button>("save-button");
        sendButton = considerationEditor.Q<Button>("send-button");

        chart = considerationEditor.Q<Chart>();

        normalizeInput = considerationEditor.Q<Toggle>("normalize-input");

        minValue = considerationEditor.Q<FloatField>("min-value");
        maxValue = considerationEditor.Q<FloatField>("max-value");

        // Populate the dropdown with the curve types
        var curves = Curve.GetCurves();
        var curveNames = new List<string>();
        foreach (var curve in curves)
        {
            curveNames.Add(curve.GetType().Name);
        }
        curveDropdown.choices = curveNames;
        Subscribe();
    }
}
