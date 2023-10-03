#if UNITY_EDITOR
using ArtificialIntelligence.Utility;
using System;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(UtilityConsideration)), CanEditMultipleObjects]
public class ConsiderationDrawer : Editor
{
    SerializedProperty showBookends;
    SerializedProperty minBookend;
    SerializedProperty maxBookend;

    SerializedProperty Implementation;

    void OnEnable()
    {
        showBookends = serializedObject.FindProperty("_bookends");
        minBookend = serializedObject.FindProperty("_minValue");
        maxBookend = serializedObject.FindProperty("_maxValue");
        Implementation = serializedObject.FindProperty("ImplementationReference");

    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(showBookends);

        if (showBookends.boolValue)
        {
            EditorGUILayout.PropertyField(minBookend);
            EditorGUILayout.PropertyField(maxBookend);
        }

        EditorGUILayout.PropertyField(Implementation);
        // Make a dropdown with all the methods
        UtilityConsideration consideration = (UtilityConsideration)target;
        GUIContent arrayList = new()
        {
            text = "Evaluation method",
            tooltip = "What piece of information from the game is this consideration evaluating"
        };
        consideration._selectedMethodIndex = EditorGUILayout.Popup(arrayList, consideration._selectedMethodIndex, consideration._methods.ToArray());

        string[] availableCurvesNames = Curve.GetCurves().Select(x => x.GetType().Name).ToArray();
        EditorGUI.BeginChangeCheck();
        consideration._selectedCurveIndex = EditorGUILayout.Popup("Curve type", consideration._selectedCurveIndex, availableCurvesNames);
        
        // Display the dropdown and detect changes
        // TODO: Duplicated code (see UtilityConsideration -> OnValidate)
        if (EditorGUI.EndChangeCheck())
        {
            consideration._curveTypes = Curve.GetCurves();
            var curveInstance = consideration._curveTypes[consideration._selectedCurveIndex];
            consideration._curve = curveInstance switch
            {
                Linear l => l,
                ExponencialInvertida ei => ei,
                Exponencial exp => exp,
                Staggered stg => stg,
                Sigmoide sigmoide => sigmoide,
                Constant constant => constant,
                Bell bell => bell,
                _ => null,
            };
        }

        // Display the properties of the selected class in the Inspector
        if (consideration._curve != null)
        {
            Type curveType = consideration._curve.GetType();
            FieldInfo[] fields = curveType.GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (FieldInfo field in fields)
            {
                if (field.FieldType == typeof(bool))
                {
                    bool boolValue = (bool)field.GetValue(consideration._curve);
                    bool newBoolValue = EditorGUILayout.Toggle(field.Name, boolValue);
                    field.SetValue(consideration._curve, newBoolValue);
                }
                else
                {

                    float currentValue = (float)field.GetValue(consideration._curve);
                    float newValue = EditorGUILayout.FloatField(field.Name, currentValue);
                    field.SetValue(consideration._curve, newValue);
                }
            }
        }
        serializedObject.ApplyModifiedProperties();
    }
}

#endif