using CBB.Lib;
using Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;


namespace CBB.ExternalTool
{
    public class EditorWindowController : MonoBehaviour
    {
        // UI
        private TreeView brainTree;
        private Button reloadBrainsButton;
        private VisualElement detailsPanel;

        // Logic
        [SerializeField]
        private BrainDataManager brainDataManager;
        private ConsiderationEditorController considerationEditorController;
        [SerializeField]
        private bool showLogs = false;
        private void Awake()
        {
            considerationEditorController = GetComponent<ConsiderationEditorController>();
            var root = GetComponent<UIDocument>().rootVisualElement;
            brainTree = root.Q<TreeView>("brain-tree");
            reloadBrainsButton = root.Q<Button>("reload-brains-button");
            detailsPanel = root.Q<VisualElement>("details-panel");
            reloadBrainsButton.clicked += LoadBrains;
            brainTree.makeItem = MakeItem;
            brainTree.bindItem = BindItem;
            brainTree.selectedIndicesChanged += OnElementSelected;
        }

        private void OnElementSelected(IEnumerable<int> enumerable)
        {
            Debug.Log($"[Editor Window Controller] Selected index: {enumerable.First()}");
            Debug.Log($"[Editor Window Controller] Selected item: {brainTree.selectedItem}");
            if (brainTree.selectedItem is DataGeneric data)
            {
                if (showLogs) Debug.Log($"[Editor Window Controller] Selected brain: {data}");
                DisplayDataGenericDetails(data);
            }
            else if (brainTree.selectedItem is ConsiderationConfiguration config)
            {
                if (showLogs) Debug.Log($"[Editor Window Controller] Selected config: {config}");
                DisplayConsiderationEditor(config);
            }
        }

        private void DisplayConsiderationEditor(ConsiderationConfiguration config)
        {
            detailsPanel.Clear();
            var ce = new ConsiderationEditor();
            considerationEditorController.SetConsiderationEditor(ce);
            considerationEditorController.ShowConsideration(config);
            detailsPanel.Add(ce);
        }

        private void DisplayDataGenericDetails(DataGeneric data)
        {
            detailsPanel.Clear();
            foreach (var item in data.Values)
            {
                // Create the visual elements for the data, asking by the type (subclass of WrapperValue)
                switch (item)
                {
                    case WraperString wraper:
                        var textField = new TextField
                        {
                            label = wraper.name,
                            value = wraper.value
                        };
                        textField.style.color = Color.white;
                        detailsPanel.Add(textField);
                        break;
                    case WraperNumber wraper:
                        var floatField = new FloatField
                        {
                            label = wraper.name,
                            value = wraper.value
                        };
                        floatField.style.color = Color.white;

                        detailsPanel.Add(floatField);
                        break;
                    case WraperBoolean wraper:
                        var toggle = new Toggle
                        {
                            label = wraper.name,
                            value = wraper.value
                        };
                        toggle.style.color = Color.white;

                        detailsPanel.Add(toggle);
                        break;
                    case WrapperConsideration wraper:
                        var gc = new GenericCard();
                        gc.SetTitle(wraper.name);
                        gc.SetSubtitle("Consideration");
                        detailsPanel.Add(gc);
                        //var considerationEditor = new ConsiderationEditor();
                        //considerationEditorController.SetConsiderationEditor(considerationEditor);
                        //considerationEditorController.ShowConsideration(wraper.configuration);
                        //detailsPanel.Add(considerationEditor);
                        break;
                    default:
                        Debug.Log($"[Editor Window Controller] WraperValue: {item}");
                        break;
                }
            }
        }

        /// <summary>
        /// Retrieve all the brains existing in the game
        /// </summary>
        public void LoadBrains()
        {
            brainTree.SetRootItems(brainDataManager.TreeRoots);
            brainTree.RefreshItems();
        }
        private void OnEnable()
        {
            LoadBrains();
        }
        private VisualElement MakeItem()
        {
            return new Label();
        }
        private void BindItem(VisualElement element, int index)
        {
            var item = element as Label;
            var itemName = brainTree.GetItemDataForIndex<IDataItem>(index).GetItemName();
            // Remove the namespace from the item name
            string pointPattern = @"[^.]*$";
            Match match = Regex.Match(itemName, pointPattern);
            if (match.Success)
            {
                // Split by capital letters. Ej: "MyBrain" -> "My", "Brain"
                string capitalPattern = @"(?=\p{Lu})";
                string[] result = Regex.Split(match.Value, capitalPattern);
                // Join the words in the array with a space
                item.text = string.Join(" ", result);
            }
            // TODO: Style the item using a uss class
            item.style.color = Color.white;
        }
    }
}
