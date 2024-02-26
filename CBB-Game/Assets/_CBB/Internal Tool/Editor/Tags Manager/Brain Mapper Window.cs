using CBB.DataManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace CBB.InternalTool
{
    public class BrainMapperWindow : EditorWindow
    {
        public VisualTreeAsset m_VisualTreeAsset = default;
        private List<BrainMap> m_brainMaps = new();
        private TextField m_agentTypeTextField;
        private TextField m_subgroupTextField;
        private ListView m_agentTypesListView;
        private ListView m_subgroupsListView;
        private Label m_subgroupTitle;
        private Button m_addAgentTypeButton;
        private Button m_removeAgentTypeButton;
        private Button m_addSubgroupButton;
        private Button m_removeSubgroupButton;

        private BrainMap m_selectedBrainMap;
        [MenuItem("CBB/Brain mapper window #&q")]
        public static void ShowExample()
        {
            BrainMapperWindow wnd = GetWindow<BrainMapperWindow>();
            wnd.titleContent = new GUIContent("Brain mapper");
        }
        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;
            m_VisualTreeAsset.CloneTree(root);

            LoadAgentTypeCollections();
            GetReferences();
            ConfigureWindow();
        }
        private void OnDestroy()
        {
            SaveTagCollections();
        }
        private void LoadAgentTypeCollections()
        {
            m_brainMaps = BrainMapsManager.GetAllBrainMaps();
            m_brainMaps ??= new();
        }
        private void GetReferences()
        {
            m_agentTypeTextField = rootVisualElement.Q<TextField>("agent-type-text-field");
            m_subgroupTextField = rootVisualElement.Q<TextField>("subgroup-text-field");
            m_agentTypesListView = rootVisualElement.Q<ListView>("agent-types-list-view");
            m_subgroupsListView = rootVisualElement.Q<ListView>("subgroups-list-view");
            m_subgroupTitle = rootVisualElement.Q<Label>("subgroup-title");
            m_addAgentTypeButton = rootVisualElement.Q<Button>("add-agent-type-button");
            m_removeAgentTypeButton = rootVisualElement.Q<Button>("remove-agent-type-button");
            m_addSubgroupButton = rootVisualElement.Q<Button>("add-subgroup-button");
            m_removeSubgroupButton = rootVisualElement.Q<Button>("remove-subgroup-button");
        }
        private void ConfigureWindow()
        {
            ConfigureSubgroupListView();
            ConfigureAgentTypeListView();
            ConfigureButtons();
            ConfigureTextFields();
        }
        private void ConfigureSubgroupListView()
        {
            var lv = m_subgroupsListView;
            lv.makeItem = () => new BrainMapItem();
            lv.bindItem = BindBrainMapItem;
        }
        private void BindBrainMapItem(VisualElement e, int index)
        {
            var item = e as BrainMapItem;
            var selectedType = m_agentTypesListView.selectedItem as BrainMap;
            item.SetData(selectedType.SubgroupsBrains[index]);
        }
        private void ConfigureAgentTypeListView()
        {
            var lv = m_agentTypesListView;
            lv.makeItem = () => new Label();
            lv.bindItem = (e, index) => (e as Label).text = m_brainMaps[index].agentType;
            lv.selectionChanged += DisplayTypeSubgroup;
            lv.itemsChosen += DisplayTypeSubgroup;
            lv.itemsSource = m_brainMaps;
            lv.RefreshItems();
        }
        private void DisplayTypeSubgroup(IEnumerable<object> item)
        {
            if (item.First() is BrainMap brainMap) m_selectedBrainMap = brainMap;
            m_subgroupsListView.itemsSource = m_selectedBrainMap.SubgroupsBrains;
            m_subgroupsListView.RefreshItems();
            m_subgroupTitle.text = $"{m_selectedBrainMap.agentType} subgroups";
        }
        public void ConfigureButtons()
        {
            m_addAgentTypeButton.clicked += AddAgentType;
            m_removeAgentTypeButton.clicked += RemoveAgentType;
            m_addSubgroupButton.clicked += AddSubgroup;
            m_removeSubgroupButton.clicked += RemoveSubgroup;
        }
        private void AddAgentType()
        {
            string name = m_agentTypeTextField.value;
            if (m_brainMaps.Count > 0)
            {
                List<string> typeNames = m_brainMaps.Select(m => m.agentType).ToList();
                if (!ItemNameIsValid(name, typeNames)) return;
            }
            BrainMap brainMap = new(name);
            m_brainMaps.Add(brainMap);
            m_agentTypesListView.RefreshItems();
            m_agentTypeTextField.value = "";
        }
        public bool ItemNameIsValid(string itemName, List<string> collection)
        {
            if (string.IsNullOrEmpty(itemName))
            {
                Debug.LogError("Name cannot be empty");
                return false;
            }
            if (collection.Contains(itemName))
            {
                Debug.LogError("Name already exists");
                return false;
            }
            return true;
        }
        private void RemoveAgentType()
        {
            var lv = m_agentTypesListView;
            if (!CanRemoveSelectedItem(lv)) return;
            var brainMap = lv.selectedItem as BrainMap;
            m_brainMaps.Remove(brainMap);
            lv.RefreshItems();
            ReloadViewAfterRemovingType();
        }
        private void ReloadViewAfterRemovingType()
        {
            if (m_brainMaps.Count == 0)
            {
                m_subgroupsListView.itemsSource = null;
                m_subgroupsListView.RefreshItems();
                m_subgroupTextField.value = "";
                return;
            }
            m_agentTypesListView.selectedIndex = 0;
            DisplayTypeSubgroup(new List<object> { m_brainMaps[0] });
        }
        public void ConfigureTextFields()
        {
            m_agentTypeTextField.RegisterCallback(OnTextFieldKeyDown(AddAgentType));
            m_subgroupTextField.RegisterCallback(OnTextFieldKeyDown(AddSubgroup));
        }
        private EventCallback<KeyDownEvent> OnTextFieldKeyDown(Action callback)
        {
            return (evt) =>
            {
                if (evt.keyCode == KeyCode.Return)
                {
                    callback();
                }
            };

        }
        private void AddSubgroup()
        {
            string name = m_subgroupTextField.value;
            List<string> subgroupNames = m_selectedBrainMap.SubgroupsBrains.Select(m => m.subgroupName).ToList();
            if (!ItemNameIsValid(name, subgroupNames)) return;
            m_selectedBrainMap.SubgroupsBrains.Add(new(name, ""));
            m_subgroupsListView.RefreshItems();
        }
        private void RemoveSubgroup()
        {
            var lv = m_subgroupsListView;
            var subgroup = lv.selectedItem as BrainMap.SubgroupBrain;
            if (!CanRemoveSelectedItem(lv)) return;
            m_selectedBrainMap.SubgroupsBrains.Remove(subgroup);
            lv.RefreshItems();
        }
        private bool CanRemoveSelectedItem(ListView listView)
        {
            if (listView == null) return false;
            if (listView.selectedIndex == -1) return false;
            return true;
        }
        private void SaveTagCollections()
        {
            BrainMapsManager.Save(m_brainMaps);
        }
    }
}