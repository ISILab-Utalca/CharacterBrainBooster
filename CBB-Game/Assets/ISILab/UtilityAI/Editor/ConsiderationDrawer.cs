#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using ArtificialIntelligence.Utility;
[CustomEditor(typeof(UtilityConsideration)), CanEditMultipleObjects]
public class ConsiderationDrawer : Editor
{
    SerializedProperty showBookends;
    SerializedProperty minBookend;
    SerializedProperty maxBookend;
    
    SerializedProperty responseCurve;
    SerializedProperty Implementation;
    void OnEnable()
    {
        showBookends = serializedObject.FindProperty("_bookends");
        minBookend = serializedObject.FindProperty("_minValue");
        maxBookend = serializedObject.FindProperty("_maxValue");
        responseCurve = serializedObject.FindProperty("_responseCurve");
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

        // Make a dropdown with all the methods
        UtilityConsideration consideration = (UtilityConsideration)target;
        EditorGUILayout.PropertyField(responseCurve);
        EditorGUILayout.PropertyField(Implementation);
        GUIContent arrayList = new()
        {
            text = "Evaluation method",
            tooltip = "What piece of information from the game is this consideration evaluating"
        };
        consideration._selectedMethodIndex = EditorGUILayout.Popup(arrayList, consideration._selectedMethodIndex, consideration._methods.ToArray());
        
        serializedObject.ApplyModifiedProperties();
    }
}

#endif