using CBB.DataManagement;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(BrainLoader))]
public class BrainLoaderDrawer : Editor
{
    /// <summary>
    /// Holds the previous state of the BrainLoader
    /// https://www.dofactory.com/net/memento-design-pattern
    /// </summary>
    class Caretaker
    {
        public BrainLoader.Memento Memento { get; set; }
    }

    Caretaker caretaker;
    DropdownField dropdown;
    TextField textField;
    public override VisualElement CreateInspectorGUI()
    {
        caretaker = new Caretaker
        {
            Memento = ((BrainLoader)serializedObject.targetObject).GetMemento()
        };

        var brainNameProperty = serializedObject.FindProperty("m_brainName");
        var agentIDProperty = serializedObject.FindProperty("m_agent_ID");

        var brainFiles = DataLoader.GetAllBrainFiles();
        var brainNames = new System.Collections.Generic.List<string>() { };


        for (int i = 0; i < brainFiles.Length; i++)
        {
            // Remove the .brain file extension from the name

            brainNames.Add(brainFiles[i].Name.Replace(".brain",""));
        }

        var container = new VisualElement();


        dropdown = new DropdownField("Select a brain", brainNames, 0)
        {
            value = brainNameProperty.stringValue
        };
        textField = new TextField("Agent ID")
        {
            value = agentIDProperty.stringValue
        };

        textField.RegisterValueChangedCallback(evt =>
        {
            agentIDProperty.stringValue = evt.newValue;
            serializedObject.ApplyModifiedProperties();
        });
        dropdown.RegisterValueChangedCallback(evt =>
        {
            brainNameProperty.stringValue = evt.newValue;
            serializedObject.ApplyModifiedProperties();
        });
        dropdown.AddToClassList(BaseField<string>.alignedFieldUssClassName);
        textField.AddToClassList(BaseField<string>.alignedFieldUssClassName);

        container.Add(textField);
        container.Add(dropdown);
        return container;
    }

    private void ApplyAndSaveChanges()
    {
        serializedObject.ApplyModifiedProperties();
        BindingManager.UpdateAgentIDBrainIDBinding(
            caretaker.Memento,
            textField.value,
            dropdown.value
            );
    }

    private void OnDisable()
    {
        ApplyAndSaveChanges();
    }
}
