using CBB.ExternalTool;
using CBB.Lib;
using Generic;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ConsiderationEditorController : MonoBehaviour
{
    #region FIELDS
    DropdownField curveDropdown;
    TextField considerationName;
    VisualElement curveParametersContainer;
    Chart chart;
    Toggle normalizeInput;
    FloatField minValue;
    FloatField maxValue;
    ExternalMonitor externalMonitor;
    ConsiderationConfiguration lastConfig = null;
    ConsiderationConfiguration originalConfig = null;
    readonly JsonSerializerSettings settings = new()
    {
        MissingMemberHandling = MissingMemberHandling.Error,
        PreserveReferencesHandling = PreserveReferencesHandling.Objects,
        TypeNameHandling = TypeNameHandling.Auto,
        Formatting = Formatting.Indented
    };

    #endregion
    private void Subscribe()
    {

        curveDropdown.RegisterValueChangedCallback(OnCurveTypeChanged);
        curveDropdown.RegisterValueChangedCallback(Utils_SetSelectedCurve);

        normalizeInput.RegisterValueChangedCallback(DisplayRange);
        normalizeInput.RegisterValueChangedCallback(Utils_SetNormalized);

        considerationName.RegisterValueChangedCallback(Utils_SetConfigurationName);
        minValue.RegisterValueChangedCallback(Utils_SetMinValue);
        maxValue.RegisterValueChangedCallback(Utils_SetmaxValue);
    }
    private void Unsubscribe()
    {

        curveDropdown.UnregisterValueChangedCallback(OnCurveTypeChanged);
        curveDropdown.UnregisterValueChangedCallback(Utils_SetSelectedCurve);

        normalizeInput.UnregisterValueChangedCallback(DisplayRange);
        normalizeInput.UnregisterValueChangedCallback(Utils_SetNormalized);

        considerationName.UnregisterValueChangedCallback(Utils_SetConfigurationName);
        minValue.UnregisterValueChangedCallback(Utils_SetMinValue);
        maxValue.UnregisterValueChangedCallback(Utils_SetmaxValue);
    }

    private void OnCurveTypeChanged(ChangeEvent<string> evt)
    {
        // Remove old data 
        curveParametersContainer.Clear();
        // Show curve parameters dynamically based on curve type
        Curve curve = Utils_GetSelectedCurve(evt.newValue);
        SetCurveParameters(curve);
        chart.SetCurve(curve);
        lastConfig.SetCurve(curve);
    }

    private void SetCurveParameters(Curve curve)
    {
        // Get the curve parameters from the curve type
        var curveParams = curve.GetGeneric().Values;
        // Add the curve parameters to the container
        foreach (var param in curveParams)
        {
            // Create the visual elements for the data by its type
            switch (param)
            {
                case WraperNumber wraper:
                    var floatField = new FloatField
                    {
                        label = wraper.name,
                        value = wraper.value
                    };
                    //floatField.RegisterValueChangedCallback((evt) => wraper.value = evt.newValue);
                    floatField.RegisterValueChangedCallback(UpdateChart);
                    floatField.style.color = Color.white;
                    floatField.labelElement.style.fontSize = 20;

                    curveParametersContainer.Add(floatField);
                    break;
                case WraperBoolean wraper:
                    var toggle = new Toggle
                    {
                        label = wraper.name,
                        value = wraper.value
                    };
                    toggle.style.color = Color.white;
                    toggle.labelElement.style.fontSize = 20;
                    //toggle.RegisterValueChangedCallback((evt) => wraper.value = evt.newValue);
                    toggle.RegisterValueChangedCallback(UpdateChart);

                    curveParametersContainer.Add(toggle);
                    break;
                case WraperString wraper:
                    var textField = new TextField
                    {
                        label = wraper.name,
                        value = wraper.value
                    };
                    // Bind the value of the text field to the value of the wrapper
                    textField.RegisterValueChangedCallback((evt) => wraper.value = evt.newValue);
                    textField.style.color = Color.white;

                    curveParametersContainer.Add(textField);
                    break;
                default:
                    Debug.Log($"[Consideration Editor Controller] Unidentified: {param}");
                    break;
            }
        }
    }

    /// <summary>
    /// Reflect the changes made to the curve's float parameters
    /// on the graphic chart
    /// </summary>
    private void UpdateChart(ChangeEvent<float> evt)
    {
        // Get the name of the field using the label of the field info
        // This works as long as the Wrappers created on runtime in SetCurveParameters
        // have the same name as the curve's fields
        string fieldname = ((FloatField)evt.currentTarget).label;
        lastConfig.curve.GetType().GetField(fieldname).SetValue(lastConfig.curve, evt.newValue);
        chart.SetCurve(lastConfig.curve);
    }
    /// <summary>
    /// Reflect the changes made to the curve's bool parameters
    /// on the graphic chart
    /// </summary>
    private void UpdateChart(ChangeEvent<bool> evt)
    {
        // Get the name of the field using the label of the field info
        // This works as long as the Wrappers created on runtime in SetCurveParameters
        // have the same name as the curve's fields
        string fieldname = ((Toggle)evt.currentTarget).label;
        lastConfig.curve.GetType().GetField(fieldname).SetValue(lastConfig.curve, evt.newValue);
        chart.SetCurve(lastConfig.curve);
    }

    
    private void ShowConsideration(ConsiderationConfiguration config)
    {
        chart.SetCurve(config.curve);
        curveDropdown.value = config.curve.GetType().Name;
        considerationName.value = config.name;
        minValue.value = config.minValue;
        maxValue.value = config.maxValue;
        normalizeInput.value = config.normalizeInput;
        SetCurveParameters(config.curve);
    }
    private void SetLocalEditorReferences(ConsiderationEditor considerationEditor)
    {
        considerationName = considerationEditor.Q<TextField>("consideration-name");
        curveDropdown = considerationEditor.Q<DropdownField>("curve-type-dropdown");
        curveParametersContainer = considerationEditor.Q<VisualElement>("curve-parameters-container");

        chart = considerationEditor.Q<Chart>();
        normalizeInput = considerationEditor.Q<Toggle>("normalize-input");

        minValue = considerationEditor.Q<FloatField>("min-value");
        maxValue = considerationEditor.Q<FloatField>("max-value");
    }

    // Creates a new consideration editor with the given consideration configuration
    public ConsiderationEditor CreateEditor(ConsiderationConfiguration config)
    {
        // Create a new consideration editor
        ConsiderationEditor considerationEditor = new();
        // Cache references to UI elements (needed for controller)
        SetLocalEditorReferences(considerationEditor);
        // Populate the dropdown with the curve types
        SetCurvesDropdown();
        // Subscribe to events (keep track of user changes)
        Subscribe();
        // Display the consideration configuration on the editor
        ShowConsideration(config);
        // Cache the original config in case the user wants to undo changes
        originalConfig = config;
        // Copy where changes are applied
        lastConfig = config;
        return considerationEditor;
    }

    #region UTILS METHODS
    // Populate the dropdown with the curve types
    private void SetCurvesDropdown()
    {
        //TODO: Get the curve types from the game, not directly from the curve class
        var curves = Curve.GetCurves();
        var curveNames = new List<string>();
        foreach (var curve in curves)
        {
            curveNames.Add(curve.GetType().Name);
        }
        curveDropdown.choices = curveNames;
    }
    /// <summary>
    /// Undo all changes made to the original consideration configuration
    /// </summary>
    public void ResetChanges()
    {
        ShowConsideration(originalConfig);
        lastConfig = originalConfig;
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

    private void Utils_SetmaxValue(ChangeEvent<float> evt)
    {
        lastConfig?.SetMaxValue(evt.newValue);
    }

    private void Utils_SetMinValue(ChangeEvent<float> evt)
    {
        lastConfig?.SetMinValue(evt.newValue);
    }
    private void Utils_SetNormalized(ChangeEvent<bool> evt)
    {
        lastConfig?.SetNormalized(evt.newValue);
    }
    void Utils_SetConfigurationName(ChangeEvent<string> evt)
    {
        // Don't do anything if the name is empty
        if (evt.newValue.Equals(string.Empty)) return;

        lastConfig?.SetName(evt.newValue);
    }
    /// <summary>
    /// Saves the selected curve on the last configuration
    /// </summary>
    /// <param name="evt">The new selected curve</param>
    void Utils_SetSelectedCurve(ChangeEvent<string> evt)
    {
        var curveType = Utils_GetSelectedCurve(evt.newValue);
        lastConfig?.SetCurve(curveType);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="curveType"></param>
    /// <returns>A new curve's type instance</returns>
    Curve Utils_GetSelectedCurve(string curveType)
    {
        //TODO: Get the curve types from the game, not directly from the curve class
        return Curve.GetCurves().Find(x => x.GetType().Name == curveType);
    }
    #endregion
}