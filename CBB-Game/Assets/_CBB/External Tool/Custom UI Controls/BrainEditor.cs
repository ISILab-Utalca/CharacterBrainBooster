using ArtificialIntelligence.Utility;
using CBB.Api;
using CBB.ExternalTool;
using CBB.Lib;
using Generic;
using System;
using System.Collections.Generic;
using System.Linq;
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
        Action addButtonCallback;

        // From Figma
        Color colorOrange = new(243f / 255, 120f / 255, 6f / 255);
        Color colorBlue = new(26f / 255, 80f / 255, 183f / 255);
        #endregion

        #region Properties
        private Brain LastSelectedBrain { get; set; } = new Brain();
        public IList<TreeViewItemData<IDataItem>> TreeRoots
        {
            get
            {
                if (Brains == null) return null;
                int id = 0;
                // First level: brains
                var roots = new List<TreeViewItemData<IDataItem>>(Brains.Count);
                foreach (var brain in Brains)
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
        private VisualElement AddButtonContainer { get; set; }
        private Button AddItemButton { get; set; }
        /// <summary>
        /// All the actions that an agent can perform.
        /// </summary>
        public List<DataGeneric> Actions { get; set; } = new();
        /// <summary>
        /// All the sensors that an agent can use.
        /// </summary>
        public List<DataGeneric> Sensors { get; set; } = new();
        public List<Brain> Brains { get; set; } = new();
        private Button AddBrainButton { get; set; }
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
            AddButtonContainer = this.Q<VisualElement>("button-add-container");
            AddItemButton = AddButtonContainer.Q<Button>("button-add");
            AddBrainButton = this.Q<Button>("add-brains-button");

            ReloadButton.clicked += ResetBrainTree;
            BrainTree.makeItem = MakeItem;
            BrainTree.bindItem = BindItem;
            AddBrainButton.clickable.clicked += () =>
            {
                var newBrain = new Brain
                {
                    brain_Name = "New Brain"
                };
                Brains.Add(newBrain);
                ResetTreeAndDisplayItem(newBrain);
            };

            BrainTree.selectedIndicesChanged += OnElementSelected;
            HandleFloatingPanel();

        }

        private void HandleFloatingPanel()
        {
            // Remove any FloatingPanel that is currently open if the user clicks
            // on any other element that is not a FloatingPanel or a FloatingPanelListItem
            this.RegisterCallback<MouseDownEvent>(evt =>
            {
                var evtFP = evt.target as FloatingPanel;
                var evtFPIL = evt.target as FloatingPanelListItem;
                if (evtFP != null || evtFPIL != null) return;

                CloseFloatingPanels();
            });
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
            uiElement.text = HelperFunctions.RemoveNamespaceSplit(itemName);
            uiElement.style.color = Color.white;
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
            DisplayItem(index, item);
        }
        private void DisplayConsiderationEditor(ConsiderationConfiguration config, int index)
        {
            DetailsPanel.Clear();
            var ce = new ConsiderationEditor(config, EvaluationMethods);
            ce.ConsiderationName.RegisterValueChangedCallback(SyncText(config, index));
            DetailsPanel.Add(ce);
            AddButtonContainer.SetDisplay(false);
        }
        private void DisplayDataGenericDetails(DataGeneric data, int index)
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
                        textField.AddToClassList("cbb-text-field");
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
                        var gc = new GenericCard(wraper);
                        gc.SetTitle(wraper.configuration.considerationName);
                        gc.SetSubtitleText("Consideration");
                        gc.SetSubtitleColor(colorBlue);
                        gc.DeleteElement += (obj) =>
                        {
                            if (obj is WrapperConsideration wc)
                            {
                                data.Values.Remove(wc);
                                ResetBrainTree();
                            }
                        };
                        DetailsPanel.Add(gc);
                        break;
                    default:
                        Debug.Log($"[Editor Window Controller] Unidentified: {item}");
                        break;
                }
            }
            switch (data.GetDataType())
            {
                case DataGeneric.DataType.Action:
                    int elementID = BrainTree.GetIdForIndex(index);
                    AddItemButton.text = "Add new consideration";
                    SetUpButton(AddItemButton, AddConsideration(data, elementID));
                    break;
                case DataGeneric.DataType.Sensor:
                    AddButtonContainer.SetDisplay(false);
                    break;
                default:
                    break;
            }
        }
        private void DisplayItemWrapperDetails(ItemWrapper wrapper, int index)
        {
            DetailsPanel.Clear();
            // Get the children of the wrapper
            var childrenIDs = BrainTree.GetChildrenIdsForIndex(index);
            // Get the data of the children
            var childrenData = childrenIDs.Select(id => BrainTree.GetItemDataForId<IDataItem>(id));

            var subTitle = wrapper.GetItemName();
            AddItemButton.text = "Add " + subTitle;
            AddButtonContainer.SetDisplay(true);
            foreach (var child in childrenData)
            {
                var gc = new GenericCard(child.GetInstance());
                gc.SetTitle(HelperFunctions.RemoveNamespaceSplit(child.GetItemName()));
                gc.SetSubtitleText(subTitle);
                gc.SetSubtitleColor(colorOrange);
                // Add logic to delete the element from the brain
                gc.DeleteElement += (obj) =>
                {
                    if (childrenData.Count() <= 1)
                    {
                        // TODO: Show a message to the user using a custom dialog
                        Debug.LogWarning($"This brain {LastSelectedBrain.brain_ID} won't work correctly without {subTitle}");
                    }
                    if (obj is DataGeneric generic)
                    {
                        switch (generic.GetDataType())
                        {
                            case DataGeneric.DataType.Action:
                                LastSelectedBrain.serializedActions.Remove(generic);
                                Debug.Log($"[Editor Window Controller] Removed action: {generic}");
                                break;
                            case DataGeneric.DataType.Sensor:
                                LastSelectedBrain.serializedSensors.Remove(generic);
                                Debug.Log($"[Editor Window Controller] Removed sensor: {generic}");
                                break;
                            default:
                                break;
                        }
                        ResetBrainTree();
                    }
                };
                DetailsPanel.Add(gc);
            }
            var parentElementID = BrainTree.GetIdForIndex(index);
            switch (subTitle)
            {
                case "Actions":
                    SetUpButton(AddItemButton, AddNewItem(parentElementID, LastSelectedBrain.serializedActions, Actions));
                    break;
                case "Sensors":
                    SetUpButton(AddItemButton, AddNewItem(parentElementID, LastSelectedBrain.serializedSensors, Sensors));
                    break;
                default:
                    break;
            }
        }
        private void DisplayBrainDetails(Brain brain, int index)
        {
            DetailsPanel.Clear();
            var textField = new TextField
            {
                label = "Brain Name",
                value = brain.brain_Name
            };
            textField.AddToClassList("cbb-text-field");
            textField.RegisterValueChangedCallback(SyncText(brain, index));
            DetailsPanel.Add(textField);
            AddButtonContainer.SetDisplay(false);
        }

        private EventCallback<ChangeEvent<string>> SyncText(INameable item, int index)
        {
            return (evt) =>
            {
                item.SetName(evt.newValue);
                BrainTree.RefreshItem(index);
            };
        }

        private void DisplayItem(int index, object item)
        {
            switch (item)
            {
                case Brain brain:
                    if (ShowLogs) Debug.Log($"[Editor Window Controller] Selected brain: {brain}");
                    DisplayBrainDetails(brain, index);
                    break;
                case ConsiderationConfiguration config:
                    if (ShowLogs) Debug.Log($"[Editor Window Controller] Selected config: {config}");
                    DisplayConsiderationEditor(config, index);
                    break;
                case ItemWrapper wrapper:
                    if (ShowLogs) Debug.Log($"[Editor Window Controller] Selected item wrapper: {wrapper}");
                    DisplayItemWrapperDetails(wrapper, index);
                    break;
                case DataGeneric data:
                    if (ShowLogs) Debug.Log($"[Editor Window Controller] Selected data generic: {data}");
                    DisplayDataGenericDetails(data, index);
                    break;
                default:
                    break;
            }
        }

        private Action AddNewItem(int parentActionID,
                                  List<DataGeneric> modifiedCollection,
                                  List<DataGeneric> sourceItems)
        {
            return () =>
            {
                CloseFloatingPanels();
                var floatingPanel = new FloatingPanel(sourceItems, this);
                floatingPanel.ElementClicked += (data) =>
                {
                    modifiedCollection.Add(data);
                    ResetTreeAndDisplayItem(data);
                    BrainTree.SetSelectionById(parentActionID);
                };
                // Adjust the panel position to be right below the add button
                floatingPanel.SetUpPosition(AddItemButton.worldBound);
                this.Add(floatingPanel);
            };
        }
        private System.Action AddConsideration(DataGeneric lastSelectedAction, int id)
        {
            // Steps:
            // Add a new wraper consideration to the action's values
            // Rebuild the treeView
            // Display the consideration editor
            return () =>
            {

                var newConsideration = new WrapperConsideration
                {
                    configuration = new ConsiderationConfiguration(
                    "New Consideration",
                    //TODO: Make sure that EvaluationMethods is not null and has at least one element, injected
                    //from the brain editor
                    new Linear(),
                    EvaluationMethods[0],
                    false,
                    0,
                    0
                    )
                };
                lastSelectedAction.Values.Add(newConsideration);

                ResetTreeAndDisplayItem(newConsideration.configuration);
                BrainTree.SetSelectionById(id);
                Debug.Log("Add consideration");
            };
        }
        public void DisplayReceivedBrains(List<Brain> brains)
        {
            SetBrains(brains);
            ResetBrainTree();
        }
        public void SetBrains(List<Brain> brains)
        {
            this.Brains = brains;
        }
        public void SetActions(List<DataGeneric> actions)
        {
            this.Actions = actions;
        }
        public void SetSensors(List<DataGeneric> sensors)
        {
            this.Sensors = sensors;
        }
        public void SetEvaluationMethods(List<string> methods)
        {
            this.EvaluationMethods = methods;
        }
        private void SetUpButton(Button button, Action newCallback)
        {
            button.clicked -= addButtonCallback;
            button.clicked += newCallback;
            addButtonCallback = newCallback;
            AddButtonContainer.SetDisplay(true);
        }

        private void ResetTreeAndDisplayItem(object item)
        {
            var lastSelectedObject = BrainTree.selectedItem;
            var lastSelectedIndex = BrainTree.selectedIndex;

            ResetBrainTree();

            if (item != null) DisplayItem(lastSelectedIndex, item);
            else DisplayItem(lastSelectedIndex, lastSelectedObject);
        }
        public void ResetBrainTree()
        {
            BrainTree.Clear();
            BrainTree.SetRootItems(TreeRoots);
            BrainTree.Rebuild();
        }
        private void CloseFloatingPanels()
        {
            var floatingPanels = this.Q<FloatingPanel>();
            floatingPanels?.RemoveFromHierarchy();
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
        public object GetInstance() => this;
    }
}
