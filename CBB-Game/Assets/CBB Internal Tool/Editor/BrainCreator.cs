using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CBB.InternalTool
{
    public class BrainCreator : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset = default;

        private ObjectField m_BrainFileField = default;
        private Toggle m_CreatePairToggle = default;
        [MenuItem("CBB/BrainCreator")]
        public static void ShowExample()
        {
            BrainCreator wnd = GetWindow<BrainCreator>();
            wnd.titleContent = new GUIContent("Brain creator");
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Instantiate UXML
            VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
            root.Add(labelFromUXML);

            // Get the create pair toggle
            m_CreatePairToggle = root.Q<Toggle>("create-pair-toggle");

            // Get the create button
            var createButton = root.Q<Button>("create-brain-button");
            createButton.clickable.clicked += CreateBrainFile;

            // Get the brain file field
            m_BrainFileField = root.Q<ObjectField>("brain-object");
        }

        private void CreateBrainFile()
        {
            if (m_BrainFileField.value == null)
            {
                Debug.LogWarning("No brain file selected.");
                return;
            }
            // The object holding the components
            var go = (GameObject)m_BrainFileField.value;
            // Get the brain loader component
            if (!go.TryGetComponent<BrainLoader>(out var brainLoader))
            {
                Debug.LogWarning("No brain loader component found.");
                return;
            }
            // Call the create brain method of the brain loader, using the name of this brain
            var b = brainLoader.CreateBrainFile();
            // Save the brain file
            DataLoader.SaveBrain(brainLoader.agent_ID, b);
            // Replace the pair in the table
            DataLoader.ReplacePair(new PairBrainData.PairBrain() { 
                agent_ID = brainLoader.agent_ID, brain_ID = b.brain_ID }, 
                m_CreatePairToggle.value);
            DataLoader.SaveTable(DataLoader.Path);
        }
    }
}
