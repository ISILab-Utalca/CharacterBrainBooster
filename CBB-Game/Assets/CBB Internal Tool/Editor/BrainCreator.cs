using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using CBB.UI;
using ArtificialIntelligence.Utility;
using System.Linq;
using Generic;
using System.Collections.Generic;
namespace CBB.InternalTool
{
    public class BrainCreator : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset = default;
        private ObjectField m_BrainFileField = default;
        private Toggle m_CreatePairToggle = default;
        private Toggle showLogsToggle;


        [MenuItem("CBB/Brain Creator")]
        public static void ShowTool()
        {
            BrainCreator wnd = GetWindow<BrainCreator>();
            wnd.titleContent = new GUIContent("Brain creator");
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            // This way of instantiating a UXML file disables the creation of
            // an additional template container in the hierarchy, which is the
            // source of problems related to different layouts between UI Builder
            // and runtime.
            m_VisualTreeAsset.CloneTree(root);

            showLogsToggle = root.Q<Toggle>("show-logs-toggle");
            m_BrainFileField = root.Q<ObjectField>("brain-object");
            m_CreatePairToggle = root.Q<Toggle>("create-pair-toggle");
            var createButton = root.Q<Button>("create-brain-button");
            createButton.clickable.clicked += CreateBrainFile;

            BrainEditor brainEditor = root.Q<BrainEditor>();
            root.Add(brainEditor);
            showLogsToggle.RegisterValueChangedCallback((evt) =>
            {
                Debug.Log(evt.newValue);
                brainEditor.ShowLogs = evt.newValue;
            });

            // Load brains and display them in the editor
            LoadBrainsInto(brainEditor);
            LoadEvaluationMethods(brainEditor);
            LoadFromGeneric<ActionState>(brainEditor.Actions);
            LoadFromGeneric<Sensor>(brainEditor.Sensors);
            
            brainEditor.SaveBrainButton.clicked += () =>
            {
                foreach (var b in brainEditor.Brains)
                {
                    DataLoader.SaveBrain(b.brain_ID, b, false);
                }
                LoadBrainsInto(brainEditor);
                brainEditor.ResetBrainTree();
            };
        }

        public static void LoadFromGeneric<T>(List<DataGeneric> container) where T : class
        {
            container.Clear();
            // Find all derived from T
            var dataGenericImplementators = HelperFunctions.GetInheritedClasses<T>();
            var dummygameObject = new GameObject();
            // Get all the data generic from the actions
            foreach (var item in dataGenericImplementators)
            {
                // In this case, al items are monobehaviours
                var instance = dummygameObject.AddComponent(item);
                // Get the data generic from the action
                var data = ((IGeneric)instance).GetGeneric();
                // Add the data generic to the brain editor
                container.Add(data);
            }
            DestroyImmediate(dummygameObject);
        }
        private static void LoadEvaluationMethods(BrainEditor brainEditor)
        {
            // Make a list of the names of the available methods for a consideration
            var cm = ConsiderationMethods.GetAllMethods();
            var methodNames = cm.Select(m => m.Name).ToList();
            brainEditor.EvaluationMethods = methodNames;
        }
        private void LoadBrainsInto(BrainEditor be) => be.SetBrains(DataLoader.GetAllBrains());
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
            DataLoader.ReplacePair(new PairBrainData.PairBrain()
            {
                agent_ID = brainLoader.agent_ID,
                brain_ID = b.brain_ID
            },
                m_CreatePairToggle.value);
            DataLoader.SaveTable(DataLoader.Path);
        }
    }
}
