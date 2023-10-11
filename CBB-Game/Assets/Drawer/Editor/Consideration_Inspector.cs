using ArtificialIntelligence.Utility;
using CBB.ExternalTool;
using System;
using System.Linq;
using System.Reflection;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(UtilityConsideration))]
public class Consideration_Inspector : Editor
{
    public VisualTreeAsset considerationVisualTree;

    private FloatField lowerBound;
    private FloatField upperBound;
    private Toggle showBounds;
    private Chart considerationGraph;
    private DropdownField methodDropdown;
    private DropdownField curveDropdown;
    private VisualElement curvePropertiesContainer;

    private UtilityConsideration consideration;
    public override VisualElement CreateInspectorGUI()
    {
        // Create a new VisualElement to be the root of our inspector UI
        VisualElement myInspector = new();
        considerationVisualTree.CloneTree(myInspector);

        // Set references
        consideration = ((UtilityConsideration)target);
        lowerBound = myInspector.Q<FloatField>("lower-bound");
        upperBound = myInspector.Q<FloatField>("upper-bound");
        showBounds = myInspector.Q<Toggle>("bookends");
        considerationGraph = myInspector.Q<Chart>();
        methodDropdown = myInspector.Q<DropdownField>("evaluation-methods");
        curveDropdown = myInspector.Q<DropdownField>("curves-dropdown");
        curvePropertiesContainer = myInspector.Q<VisualElement>("curve-properties-container");
        considerationGraph = myInspector.Q<Chart>();

        // Make method dropdown shows the currently selected method
        methodDropdown.choices = consideration.m_methods;
        var selectedMethodIndex = serializedObject.FindProperty("_selectedMethodIndex").intValue;
        methodDropdown.index = selectedMethodIndex;

        // Make curve dropdown shows the currently selected curve
        curveDropdown.choices = Curve.GetCurves().Select(x => x.GetType().Name).ToList();
        var selectedCurveIndex = serializedObject.FindProperty("_selectedCurveIndex").intValue;
        curveDropdown.index = selectedCurveIndex;

        // Make UI show the properties of the current curve
        ManageCurveSelection(curveDropdown.choices[selectedCurveIndex]);

        // Add event handlers and logic
        showBounds.RegisterValueChangedCallback((evt) => ManageBoundsDisplay(evt.newValue));

        methodDropdown.RegisterValueChangedCallback(evt => ManageMethodChange(evt.newValue));

        curveDropdown.RegisterValueChangedCallback((evt) => ManageCurveSelection(evt.newValue));

        //Debug.Log("method index: " + );
        // Return the finished inspector UI
        serializedObject.ApplyModifiedProperties();
        return myInspector;
    }

    private void ManageMethodChange(string newValue)
    {
        var methodIndex = methodDropdown.choices.IndexOf(newValue);

        if (methodIndex != -1)
        {
            consideration.UpdateMethodInfo(methodIndex);
            methodDropdown.value = newValue;
        }
        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();
    }

    private void ManageCurveSelection(string newValue)
    {
        serializedObject.Update();
        var curveIndex = curveDropdown.choices.IndexOf(newValue);
        consideration.ResetCurves(curveIndex);
        considerationGraph.SetCurve(consideration._curve, 0);
        if (consideration._curve != null)
        {
            UpdateCurvePropertiesUI(curvePropertiesContainer);
        }
        serializedObject.ApplyModifiedProperties();
    }
    private void ManageBoundsDisplay(bool value)
    {
        lowerBound.SetDisplay(value);
        upperBound.SetDisplay(value);
    }

    private void UpdateCurvePropertiesUI(VisualElement container)
    {
        container.Clear();
        //var curveSerializedObject = new SerializedObject(consideration._curve);
        //var property = curveSerializedObject.GetIterator();
        //while (property.NextVisible(true))
        //{
        //    var propertyName = property.displayName;
        //    if (property.propertyType == SerializedPropertyType.Float)
        //    {
        //        var field = new FloatField(propertyName)
        //        {
        //            value = property.floatValue,
        //        };
        //        field.RegisterValueChangedCallback(evt =>
        //        {
        //            property.floatValue = evt.newValue;
        //            curveSerializedObject.ApplyModifiedProperties();
        //            EditorUtility.SetDirty(target);
        //        });
        //        container.Add(field);
        //    }
        //    else if (property.propertyType == SerializedPropertyType.Integer)
        //    {
        //        var field = new IntegerField(propertyName)
        //        {
        //            value = property.intValue
        //        };
        //        field.RegisterValueChangedCallback(evt =>
        //        {
        //            property.intValue = evt.newValue;
        //            EditorUtility.SetDirty(target);
        //            serializedObject.ApplyModifiedProperties();
        //        });
        //        container.Add(field);
        //    }
        //    else if (property.propertyType == SerializedPropertyType.Boolean)
        //    {
        //        var field = new Toggle(propertyName)
        //        {
        //            value = property.boolValue
        //        };
        //        field.RegisterValueChangedCallback(evt =>
        //        {
        //            property.boolValue = evt.newValue;
        //            EditorUtility.SetDirty(target);
        //            serializedObject.ApplyModifiedProperties();
        //        });
        //        container.Add(field);
        //    }
        //}

        var curve = consideration._curve;
        Type curveType = curve.GetType();

        //Iterate through the properties of the derived Curve type using reflection
        var properties = curveType.GetFields(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            var propertyName = ObjectNames.NicifyVariableName(property.Name);
            var propertyValue = property.GetValue(curve);
            if (property.FieldType == typeof(float))
            {
                var field = new FloatField(propertyName)
                {
                    value = (float)propertyValue
                };
                field.RegisterValueChangedCallback(evt =>
                {
                    property.SetValue(curve, evt.newValue);
                    EditorUtility.SetDirty(target);
                    serializedObject.ApplyModifiedProperties();
                });
                container.Add(field);
            }
            else if (property.FieldType == typeof(int))
            {
                var field = new IntegerField(propertyName)
                {
                    value = (int)propertyValue
                };
                field.RegisterValueChangedCallback(evt =>
                {
                    property.SetValue(curve, evt.newValue);
                    EditorUtility.SetDirty(target);
                    serializedObject.ApplyModifiedProperties();
                });
                container.Add(field);
            }
            else if (property.FieldType == typeof(bool))
            {
                var field = new Toggle(propertyName)
                {
                    value = (bool)propertyValue
                };
                field.RegisterValueChangedCallback(evt =>
                {
                    property.SetValue(curve, evt.newValue);
                    EditorUtility.SetDirty(target);
                    serializedObject.ApplyModifiedProperties();
                });
                container.Add(field);
            }
            // Add more field types as needed
        }
    }
}
