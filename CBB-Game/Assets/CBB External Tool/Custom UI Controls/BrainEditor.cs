using ArtificialIntelligence.Utility;
using CBB.ExternalTool;
using CBB.Lib;
using Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

namespace CBB.UI

{
    public partial class BrainEditor : VisualElement
    {
        #region FACTORY
        public new class UxmlFactory : UxmlFactory<BrainEditor, UxmlTraits> { }

        #endregion

        #region Fields
        List<Brain> brains = new();
        List<System.Action> reloadButtonCallbacks = new();

        // From Figma
        Color colorOrange = new(243f / 255, 120f / 255, 6f / 255);
        Color colorBlue = new(26f / 255, 80f / 255, 183f / 255);
        #endregion

        #region Properties
        public Brain LastSelectedBrain { get; private set; }
        public IList<TreeViewItemData<IDataItem>> TreeRoots
        {
            get
            {
                if (brains == null) return null;
                int id = 0;
                // First level: brains
                var roots = new List<TreeViewItemData<IDataItem>>(brains.Count);
                foreach (var brain in brains)
                {
                    // This is an intermediate list to store the sensors and actions of the brain
                    var brainDataGroup = new List<TreeViewItemData<IDataItem>>();

                    // Second level: actions and sensors
                    var actionWrapper = new ItemWrapper("Actions");
                    var actionItems = new List<TreeViewItemData<IDataItem>>();
                    foreach (var action in brain.serializedActions)
                    {
                        // Third level: considerations attached to this action
                        var considerationItems = new List<TreeViewItemData<IDataItem>>();
                        foreach (var value in action.Values)
                        {
                            if (value is WrapperConsideration wc)
                            {
                                considerationItems.Add(new TreeViewItemData<IDataItem>(id++, wc.configuration));
                            }
                        }
                        //considerationItems.Add(new TreeViewItemData<IDataItem>(id++,new ButtonItem()));
                        actionItems.Add(new TreeViewItemData<IDataItem>(id++, action, considerationItems));
                    }

                    var sensorWrapper = new ItemWrapper("Sensors");
                    var sensorItems = new List<TreeViewItemData<IDataItem>>(brain.serializedSensors.Count);
                    foreach (var sensor in brain.serializedSensors)
                    {
                        sensorItems.Add(new TreeViewItemData<IDataItem>(id++, sensor));
                    }

                    // Production version, with children
                    brainDataGroup.Add(new TreeViewItemData<IDataItem>(id++, actionWrapper, actionItems));
                    brainDataGroup.Add(new TreeViewItemData<IDataItem>(id++, sensorWrapper, sensorItems));
                    roots.Add(new TreeViewItemData<IDataItem>(id++, brain, brainDataGroup));
                }
                return roots;
            }
        }
        private VisualElement DetailsPanel { get; set; }
        private Button ReloadButton { get; set; }
        public TreeView BrainTree { get; set; }
        public Button SaveBrainButton { get; set; }
        public bool ShowLogs { get; set; } = false;
        public List<string> EvaluationMethods { get; set; } = new();
        #endregion

        #region Constructors
        public BrainEditor()
        {
            var visualTree = Resources.Load<VisualTreeAsset>("Editor Mode/Brain Editor");

            visualTree.CloneTree(this);

            DetailsPanel = this.Q<VisualElement>("details-panel");
            ReloadButton = this.Q<Button>("reload-brains-button");
            BrainTree = this.Q<TreeView>("brain-tree");
            SaveBrainButton = this.Q<Button>("save-brain-button");

            ReloadButton.clicked += DisplayBrainsTreeView;
            BrainTree.makeItem = MakeItem;
            BrainTree.bindItem = BindItem;

            BrainTree.selectedIndicesChanged += OnElementSelected;
        }
        #endregion

        class ItemWrapper : IDataItem
        {
            readonly string name;
            public ItemWrapper(string name)
            {
                this.name = name;
            }
            public string GetItemName() => name;
            public object GetInstance() => this;
        }

        private VisualElement MakeItem()
        {
            return new Label();
        }
        private void BindItem(VisualElement element, int index)
        {
            var itemData = BrainTree.GetItemDataForIndex<IDataItem>(index);
            var uiElement = element as Label;
            var itemName = itemData.GetItemName();
            RemoveNamespace(uiElement, itemName);
        }

        // Remove the namespace from the item name
        private static void RemoveNamespace(Label uiElement, string itemName)
        {
            string pointPattern = @"[^.]*$";
            Match match = Regex.Match(itemName, pointPattern);
            if (match.Success)
            {
                // Split by capital letters. Ej: "MyBrain" -> "My", "Brain"
                string capitalPattern = @"(?=\p{Lu})";
                string[] result = Regex.Split(match.Value, capitalPattern);
                // Join the words in the array with a space
                uiElement.text = string.Join(" ", result);
            }
        }

        public void SetReloadButtonCallback(System.Action callback)
        {
            reloadButtonCallbacks.Add(callback);
            ReloadButton.clicked += callback;
        }
        public void RemoveAllReloadButtonCallbacks()
        {
            foreach (var callback in reloadButtonCallbacks)
            {
                ReloadButton.clicked -= callback;
            }
            reloadButtonCallbacks.Clear();
        }
        public void RemoveReloadButtonCallback(System.Action action)
        {
            ReloadButton.clicked -= action;
            reloadButtonCallbacks.Remove(action);
        }
        public void SetBrains(List<Brain> brains)
        {
            this.brains = brains;
            DisplayBrainsTreeView();
        }
        private void DisplayConsiderationEditor(ConsiderationConfiguration config)
        {
            DetailsPanel.Clear();
            var ce = new ConsiderationEditor(config, EvaluationMethods);
            DetailsPanel.Add(ce);
        }
        private void DisplayDataGenericDetails(DataGeneric data)
        {
            DetailsPanel.Clear();
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
                        DetailsPanel.Add(textField);
                        break;
                    case WraperNumber wraper:
                        var floatField = new FloatField
                        {
                            label = wraper.name,
                            value = wraper.value
                        };
                        floatField.RegisterValueChangedCallback((evt) => wraper.value = evt.newValue);
                        floatField.style.color = Color.white;

                        DetailsPanel.Add(floatField);
                        break;
                    case WraperBoolean wraper:
                        var toggle = new Toggle
                        {
                            label = wraper.name,
                            value = wraper.value
                        };
                        toggle.style.color = Color.white;
                        toggle.RegisterValueChangedCallback((evt) => wraper.value = evt.newValue);

                        DetailsPanel.Add(toggle);
                        break;
                    case WrapperConsideration wraper:
                        var gc = new GenericCard();
                        gc.SetTitle(wraper.name);
                        gc.SetSubtitleText("Consideration");
                        gc.SetSubtitleColor(colorBlue);
                        DetailsPanel.Add(gc);
                        break;
                    default:
                        Debug.Log($"[Editor Window Controller] Unidentified: {item}");
                        break;
                }
            }
        }
        private void DisplayBrainsTreeView()
        {
            BrainTree.SetRootItems(TreeRoots);
            BrainTree.Rebuild();
        }
        private void DisplayItemWrapperDetails(ItemWrapper wrapper, int index)
        {
            DetailsPanel.Clear();
            // Get the children of the wrapper
            var childrenIDs = BrainTree.GetChildrenIdsForIndex(index);
            // Get the data of the children
            var childrenData = childrenIDs.Select(id => BrainTree.GetItemDataForId<IDataItem>(id));

            var subTitle = wrapper.GetItemName();
            var buttonContainer = new VisualElement();

            var addButton = new Button
            {
                text = "Add " + subTitle
            };

            buttonContainer.Add(addButton);
            foreach (var child in childrenData)
            {
                var gc = new GenericCard();
                gc.SetTitle(child.GetItemName());
                gc.SetSubtitleText(subTitle);
                gc.SetSubtitleColor(colorOrange);
                DetailsPanel.Add(gc);
            }
            switch (subTitle)
            {
                case "Actions":
                    addButton.clicked += AddAction;
                    break;
                case "Sensors":
                    addButton.clicked += AddSensor;
                    break;
                default:
                    break;
            }
            DetailsPanel.Add(buttonContainer);

        }

        private void AddSensor()
        {
            throw new NotImplementedException();
        }

        private void AddAction()
        {
            throw new NotImplementedException();
        }

        private Brain GetParentBrainFromIndex(int itemIndex)
        {
            var parentId = BrainTree.GetParentIdForIndex(itemIndex);
            // We hit the root of the tree
            if (parentId == -1)
            {
                return BrainTree.GetItemDataForIndex<Brain>(itemIndex);
            }
            // Else, set the selected item by id. This forces the tree to update the selected item
            // thus being able to traverse the tree upwards, since its only possible to ask for the parent
            // of an item by index i.e. you can't ask for the parent of an item by its id, neither can
            // get the index of an item by its id, since it changes dynamically.
            // Use selection without notify to avoid triggering the selectedItemsChanged event causing an infinite loop
            BrainTree.SetSelectionByIdWithoutNotify(new List<int>() { parentId });
            return GetParentBrainFromIndex(BrainTree.selectedIndex);
        }
        private void OnElementSelected(IEnumerable<int> enumerable)
        {
            var index = enumerable.First();
            var item = BrainTree.selectedItem;

            //Cache the last selected brain for editing purposes.
            LastSelectedBrain = item is Brain b ? b : GetParentBrainFromIndex(index);

            // Reset the selection on the UI, so the user doesn't get confused
            BrainTree.SetSelectionByIdWithoutNotify(
                new List<int>() { BrainTree.GetIdForIndex(index)
                });
            if (item is DataGeneric data)
            {
                if (ShowLogs) Debug.Log($"[Editor Window Controller] Selected brain: {data}");
                DisplayDataGenericDetails(data);
            }
            else if (item is ConsiderationConfiguration config)
            {
                if (ShowLogs) Debug.Log($"[Editor Window Controller] Selected config: {config}");
                DisplayConsiderationEditor(config);
            }
            else if (item is ItemWrapper wrapper)
            {
                DisplayItemWrapperDetails(wrapper, index);
            }
        }
        public object GetInstance() => this;
        public void SetAgentActions(List<string> actions)
        {

        }
    }
}
