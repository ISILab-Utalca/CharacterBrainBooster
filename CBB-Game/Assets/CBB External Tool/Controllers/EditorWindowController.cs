using CBB.Lib;
using Generic;
using Newtonsoft.Json;
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
        private Button saveBrainButton;

        // Logic
        [SerializeField]
        private BrainDataManager brainDataManager;
        [SerializeField]
        private bool showLogs = false;
        private Brain lastSelectedBrain;
        private ConsiderationEditorController considerationEditorController;
        private readonly JsonSerializerSettings settings = new()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            // This is an important property in order to avoid type errors when deserializing
            TypeNameHandling = TypeNameHandling.Auto,
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            Formatting = Formatting.Indented
        };

        private void Awake()
        {
            // Find UI references
            considerationEditorController = GetComponent<ConsiderationEditorController>();
            var root = GetComponent<UIDocument>().rootVisualElement;
            brainTree = root.Q<TreeView>("brain-tree");
            reloadBrainsButton = root.Q<Button>("reload-brains-button");
            saveBrainButton = root.Q<Button>("save-brain-button");
            detailsPanel = root.Q<VisualElement>("details-panel");

            // Add listeners to UI elements
            reloadBrainsButton.clicked += DisplayBrainsTreeView;
            saveBrainButton.clicked += SaveBrain;
            brainTree.makeItem = MakeItem;
            brainTree.bindItem = BindItem;
            brainTree.selectedIndicesChanged += OnElementSelected;

        }
        private void SaveBrain()
        {
            var b = lastSelectedBrain;
            // Save the updated file to a local path
            string path = Application.dataPath + "/CBB/ExternalTool/Brains/" + b.brain_ID + ".json";
            string json = JsonConvert.SerializeObject(b, settings);

            System.IO.File.WriteAllText(path, json);
            Debug.Log($"[Editor Window Controller] Brain saved to: {path}");
        }
        // Recursively, finds the brain associated to the selected item
        private Brain GetParentBrain(int index)
        {
            var parentId = brainTree.GetParentIdForIndex(index);
            // We hit the root of the tree
            if (parentId == -1)
            {
                return brainTree.GetItemDataForIndex<Brain>(index);
            }
            DebugSelectedItemInfo(brainTree.selectedIndex);
            // Else, set the selected item by id. This forces the tree to update the selected item
            // thus being able to traverse the tree upwards, since its only possible to ask for the parent
            // of an item by index i.e. you can't ask for the parent of an item by its id, neither can
            // get the index of an item by its id, since it changes dynamically.
            // Use selection without notify to avoid triggering the selectedItemsChanged event causing an infinite loop
            brainTree.SetSelectionByIdWithoutNotify(new List<int>() { parentId });
            return GetParentBrain(brainTree.selectedIndex);
        }
        private void OnElementSelected(IEnumerable<int> enumerable)
        {
            var index = enumerable.First();
            var item = brainTree.selectedItem;

            //Cache the last selected brain for saving purposes.
            //You need to call GetParentBrain since the user can select any item in the tree
            lastSelectedBrain = item is Brain b ? b : GetParentBrain(index);
            
            // Reset the selection on the UI, so the user doesn't get confused
            brainTree.SetSelectionByIdWithoutNotify(new List<int>() { brainTree.GetIdForIndex(index) });
            if (item is DataGeneric data)
            {
                if (showLogs) Debug.Log($"[Editor Window Controller] Selected brain: {data}");
                DisplayDataGenericDetails(data);
            }
            else if (item is ConsiderationConfiguration config)
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
                // Create the visual elements for the data by its type
                switch (item)
                {
                    case WraperString wraper:
                        var textField = new TextField
                        {
                            label = wraper.name,
                            value = wraper.value
                        };
                        // Bind the value of the text field to the value of the wrapper
                        textField.RegisterValueChangedCallback((evt) => wraper.value = evt.newValue);
                        textField.style.color = Color.white;
                        detailsPanel.Add(textField);
                        break;
                    case WraperNumber wraper:
                        var floatField = new FloatField
                        {
                            label = wraper.name,
                            value = wraper.value
                        };
                        floatField.RegisterValueChangedCallback((evt) => wraper.value = evt.newValue);
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
                        toggle.RegisterValueChangedCallback((evt) => wraper.value = evt.newValue);

                        detailsPanel.Add(toggle);
                        break;
                    case WrapperConsideration wraper:
                        var gc = new GenericCard();
                        gc.SetTitle(wraper.name);
                        gc.SetSubtitle("Consideration");
                        detailsPanel.Add(gc);
                        break;
                    default:
                        Debug.Log($"[Editor Window Controller] Unidentified: {item}");
                        break;
                }
            }
        }
        public void DisplayBrainsTreeView()
        {
            brainTree.SetRootItems(brainDataManager.TreeRoots);
            brainTree.RefreshItems();
        }
        private void OnEnable()
        {
            DisplayBrainsTreeView();
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
        private string PrintArray(IEnumerable<int> array)
        {
            string result = "[";
            foreach (var item in array)
            {
                result += item + ", ";
            }
            result += "]";
            return result;
        }
        private void DebugSelectedItemInfo(int index)
        {
            Debug.Log($"[Editor Window Controller] Item index: {index} " +
                $"\nitem: {brainTree.selectedItem}" +
                $"\nitem type: {brainTree.selectedItem.GetType()}" +
                $"\nItem ID: {brainTree.GetIdForIndex(index)}" +
                "\nitem children: " + PrintArray(brainTree.GetChildrenIdsForIndex(index)) +
                $"\nitem parent's ID: {brainTree.GetParentIdForIndex(index)}" +
                $"\nRoot element for index {index}: {brainTree.GetRootElementForIndex(index)}"
            //$"\nitem parent's data (index): {brainTree.GetItemDataForIndex<object>(index)}" +
            //$"\nitem parent's data (ID): {brainTree.GetItemDataForId<object>(brainTree.GetParentIdForIndex(index))}"
            );
        }
    }
}
