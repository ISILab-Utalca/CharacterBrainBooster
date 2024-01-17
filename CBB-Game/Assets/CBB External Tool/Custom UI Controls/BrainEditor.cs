using ArtificialIntelligence.Utility;
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
        List<Brain> brains = new();
        Action addButtonCallback;

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
        private VisualElement AddButtonContainer { get; set; }
        private Button AddButton { get; set; }
        /// <summary>
        /// All the actions that an agent can perform.
        /// </summary>
        public List<DataGeneric> Actions { get; set; } = new();
        /// <summary>
        /// All the sensors that an agent can use.
        /// </summary>
        public List<DataGeneric> Sensors { get; set; } = new();

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
            AddButton = AddButtonContainer.Q<Button>("button-add");

            ReloadButton.clicked += DisplayBrainsTreeView;
            BrainTree.makeItem = MakeItem;
            BrainTree.bindItem = BindItem;

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
            uiElement.text = HelperFunctions.RemoveNamespace(itemName);
        }

        
        
        private void DisplayConsiderationEditor(ConsiderationConfiguration config)
        {
            DetailsPanel.Clear();
            var ce = new ConsiderationEditor(config, EvaluationMethods);
            DetailsPanel.Add(ce);
            AddButtonContainer.SetDisplay(false);
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
            switch (data.GetDataType())
            {
                case DataGeneric.DataType.Action:
                    AddButton.text = "Add new consideration";
                    SetUpButton(AddButton, AddConsideration);
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
            AddButton.text = "Add " + subTitle;
            AddButtonContainer.SetDisplay(true);
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
                    SetUpButton(AddButton, AddAction);
                    break;
                case "Sensors":
                    SetUpButton(AddButton, AddSensor);
                    break;
                default:
                    break;
            }
        }
        private void DisplayBrainDetails(Brain brain)
        {
            DetailsPanel.Clear();
            AddButtonContainer.SetDisplay(false);
        }

        private void AddSensor()
        {
            CloseFloatingPanels();
            var floatingPanel = new FloatingPanel(Sensors, this);
            // Adjust the panel position to be right below the add button
            floatingPanel.SetUpPosition(AddButton.worldBound);
            this.Add(floatingPanel);
            Debug.Log("Add sensor");

        }
        private void AddAction()
        {
            // Avoid having multiple floating panels
            CloseFloatingPanels();
            var floatingPanel = new FloatingPanel(Actions, this);
            // Adjust the panel position to be right below the add button
            floatingPanel.SetUpPosition(AddButton.worldBound);
            this.Add(floatingPanel);
            Debug.Log("Add action");
        }
        private void AddConsideration()
        {
            Debug.Log("Add consideration");
            // Steps:
            // Get the last selected action
            // Add a new wraper consideration to the action's values
            // Rebuild the treeView
            // Display the consideration editor

            var lastSelectedAction = BrainTree.selectedItem as DataGeneric;
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
            var v = TreeRoots;
            Debug.Log(v);

            BrainTree.Clear();
            BrainTree.SetRootItems(TreeRoots);
            BrainTree.Rebuild();
        }

        private void CloseFloatingPanels()
        {
            var floatingPanels = this.Q<FloatingPanel>();
            floatingPanels?.RemoveFromHierarchy();
        }
        private void SetUpButton(Button button, Action newCallback)
        {
            try
            {
                button.clicked -= addButtonCallback;
                button.clicked += newCallback;
                addButtonCallback = newCallback;
                //button.clicked -= AddSensor;
                //button.clicked -= AddAction;
                //button.clicked -= AddConsideration;

                //button.clicked += newCallback;
                AddButtonContainer.SetDisplay(true);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
        public void SetBrains(List<Brain> brains)
        {
            this.brains = brains;
            DisplayBrainsTreeView();
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
            switch (item)
            {
                case Brain brain:
                    if (ShowLogs) Debug.Log($"[Editor Window Controller] Selected brain: {brain}");
                    DisplayBrainDetails(brain);
                    break;
                case ConsiderationConfiguration config:
                    if (ShowLogs) Debug.Log($"[Editor Window Controller] Selected config: {config}");
                    DisplayConsiderationEditor(config);
                    break;
                case ItemWrapper wrapper:
                    if (ShowLogs) Debug.Log($"[Editor Window Controller] Selected item wrapper: {wrapper}");
                    DisplayItemWrapperDetails(wrapper, index);
                    break;
                case DataGeneric data:
                    if (ShowLogs) Debug.Log($"[Editor Window Controller] Selected data generic: {data}");
                    DisplayDataGenericDetails(data);
                    break;
                default:
                    break;
            }
        }
        public void SetAgentActions(List<string> actions)
        {

        }
        private void DisplayBrainsTreeView()
        {
            BrainTree.SetRootItems(TreeRoots);
            BrainTree.Rebuild();
        }
        public object GetInstance() => this;
    }
}
