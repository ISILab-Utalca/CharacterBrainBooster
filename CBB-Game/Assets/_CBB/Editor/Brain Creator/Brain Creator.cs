using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using CBB.UI;
using ArtificialIntelligence.Utility;
using System.Linq;
using Generic;
using System.Collections.Generic;
using CBB.DataManagement;

namespace CBB.InternalTool
{
    public class BrainCreator : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset = default;

        [MenuItem("Window/ISILab/Brain Editor #&w")]
        public static void ShowTool()
        {
            BrainCreator wnd = GetWindow<BrainCreator>();
            wnd.titleContent = new GUIContent("Brain Editor");
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            // This way of instantiating a UXML file disables the creation of
            // an additional template container in the hierarchy, which is the
            // source of problems related to different layouts between UI Builder
            // and runtime.
            m_VisualTreeAsset.CloneTree(root);

            BrainEditor brainEditor = root.Q<BrainEditor>();
            root.Add(brainEditor);

            // Load brains and display them in the editor
            LoadBrainsInto(brainEditor);
            brainEditor.ResetBrainTree();

            LoadEvaluationMethods(brainEditor);
            LoadFromGeneric<ActionState>(brainEditor.Actions);
            LoadFromGeneric<Sensor>(brainEditor.Sensors);
            
            brainEditor.SaveBrainsButton.clicked += () =>
            {
                foreach (var b in brainEditor.Brains)
                {
                    BrainDataLoader.SaveBrain(b);
                    BrainDataLoader.BrainUpdated?.Invoke(b);
                }
                AssetDatabase.Refresh();
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
        private void LoadBrainsInto(BrainEditor be) => be.SetBrains(BrainDataLoader.GetAllBrains());
        
    }
}
