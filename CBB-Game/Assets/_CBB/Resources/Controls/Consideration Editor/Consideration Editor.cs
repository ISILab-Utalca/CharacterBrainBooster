using ArtificialIntelligence.Utility;
using CBB.Lib;
using Generic;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace CBB.ExternalTool
{
    public class ConsiderationEditor : VisualElement
    {
        #region Factory
        public new class UxmlFactory : UxmlFactory<ConsiderationEditor, UxmlTraits> { }
        #endregion

        #region Fields
        DropdownField curveDropdown;
        DropdownField methodDropdown;
        VisualElement curveParametersContainer;
        Chart chart;
        Toggle normalizeInput;
        FloatField minValue;
        FloatField maxValue;
        ConsiderationConfiguration lastConfig = null;
        ConsiderationConfiguration originalConfig = null;

        /// <summary>
        /// Expose this field to the controller to get the name of the consideration
        /// and set the events to update the name of another items on the UI
        /// </summary>
        public TextField ConsiderationName { get; set; }
        #endregion

        #region Constructors
        public ConsiderationEditor()
        {
            var visualTree = Resources.Load<VisualTreeAsset>("Controls/Consideration Editor/Consideration Editor");
            visualTree.CloneTree(this);
        }
        public ConsiderationEditor(ConsiderationConfiguration cc) : this()
        {
            SetUpEditor(cc);
        }
        public ConsiderationEditor(ConsiderationConfiguration cc, List<string> methodNames) : this(cc)
        {
            SetEvaluationMethodsDropdown(methodNames);
        } 
        #endregion

        private void SetUpEditor(ConsiderationConfiguration config)
        {
            // Cache references to UI elements (needed for controller)
            SetLocalEditorReferences();
            // Populate dropdowns and display current values
            SetCurvesDropdown();
            DisplayCurrentEvaluationMethod(config);
            // Subscribe to events (keep track of user changes)
            Subscribe();
            // Display the consideration configuration on the editor
            ShowConsideration(config);
            // Display the min-max values if config says so
            DisplayRange(config.normalizeInput);
            // Cache the original config in case the user wants to undo changes
            originalConfig = config;
            // Copy where changes are applied
            lastConfig = config;
        }
        private void SetLocalEditorReferences()
        {
            ConsiderationName = this.Q<TextField>("consideration-name");
            curveDropdown = this.Q<DropdownField>("curve-type-dropdown");
            methodDropdown = this.Q<DropdownField>("evaluation-method-dropdown");
            curveParametersContainer = this.Q<VisualElement>("curve-parameters-container");

            chart = this.Q<Chart>();
            normalizeInput = this.Q<Toggle>("normalize-input");

            minValue = this.Q<FloatField>("min-value");
            maxValue = this.Q<FloatField>("max-value");
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
        private void DisplayCurrentEvaluationMethod(ConsiderationConfiguration cc)
        {
#if UNITY_STANDALONE_WIN
            // Make a list of the names of the available methods for a consideration
            var cm = ConsiderationMethods.GetAllMethods();
            var methodNames = cm.Select(m => m.Name).ToList();
            methodDropdown.choices = methodNames;
#endif
            methodDropdown.value = cc.evaluationMethod;
        }
        private void Subscribe()
        {

            curveDropdown.RegisterValueChangedCallback(OnCurveTypeChanged);
            curveDropdown.RegisterValueChangedCallback(Utils_SetSelectedCurve);

            methodDropdown.RegisterValueChangedCallback(OnEvaluationMethodChanged);

            normalizeInput.RegisterValueChangedCallback(DisplayRange);
            normalizeInput.RegisterValueChangedCallback(Utils_SetNormalized);

            ConsiderationName.RegisterValueChangedCallback(Utils_SetConfigurationName);
            minValue.RegisterValueChangedCallback(Utils_SetMinValue);
            maxValue.RegisterValueChangedCallback(Utils_SetmaxValue);
        }
        private void OnEvaluationMethodChanged(ChangeEvent<string> evt)
        {
            //NOTE: since "lastConfig" is a reference to a TreeView item, the changes are applied
            //to the original item and saved correctly when serializing the associated brain object
            lastConfig.evaluationMethod = evt.newValue;
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
        private void UpdateChart(ChangeEvent<float> evt)
        {
            // Get the name of the field using the label of the field info
            // This works as long as the Wrappers created on runtime in SetCurveParameters
            // have the same name as the curve's fields
            string fieldname = ((FloatField)evt.currentTarget).label;
            lastConfig.curve.GetType().GetField(fieldname).SetValue(lastConfig.curve, evt.newValue);
            chart.SetCurve(lastConfig.curve);
        }
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
            ConsiderationName.value = config.considerationName;
            minValue.value = config.minValue;
            maxValue.value = config.maxValue;
            normalizeInput.value = config.normalizeInput;
            SetCurveParameters(config.curve);
        }

        #region UTILS METHODS
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
        public void SetEvaluationMethodsDropdown(List<string> methodNames)
        {
            methodDropdown.choices = methodNames;
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
            DisplayRange(evt.newValue);
        }
        private void DisplayRange(bool b)
        {
            minValue.style.display = b ? DisplayStyle.Flex : DisplayStyle.None;
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
}

