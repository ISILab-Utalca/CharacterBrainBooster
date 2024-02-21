using CBB.InternalTool;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace CBB.DataManagement
{
    public class TagsManagerWindow : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset = default;
        private List<TagCollection> m_collections;

        private CollectionPanel m_agentTypesPanel;
        private CollectionPanel m_typesSubgroupsPanel;

        [MenuItem("CBB/Tags Manager Window #&q")]
        public static void ShowExample()
        {
            TagsManagerWindow wnd = GetWindow<TagsManagerWindow>();
            wnd.titleContent = new GUIContent("TagsManagerWindow");
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;
            VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
            root.Add(labelFromUXML);

            GetReferences();
            LoadAgentTypeCollections();
            ConfigurePanels();
        }
        private void OnDestroy()
        {
            SaveTagCollections();
        }
        private void GetReferences()
        {
            m_agentTypesPanel = rootVisualElement.Q<CollectionPanel>("agent-types-collection-panel");
            m_typesSubgroupsPanel = rootVisualElement.Q<CollectionPanel>("types-subgroups-collection-panel");
        }
        private void LoadAgentTypeCollections()
        {
            m_collections = TagsManager.GetAllCollections();
        }
        private void ConfigurePanels()
        {
            var lv = m_agentTypesPanel.ListView;
            lv.bindItem = (e, index) => (e as Label).text = m_collections[index].name;
            lv.selectionChanged += SelectAgentType;
            lv.itemsChosen += SelectAgentType;
            lv.itemsSource = m_collections;
            lv.RefreshItems();

            m_agentTypesPanel.AddItemButton.clicked += AddAgentType;
            m_agentTypesPanel.RemoveItemButton.clicked += RemoveAgentType;
            m_agentTypesPanel.RegisterCallbackOnTextFieldEnterPressed(AddAgentType);
            
            m_typesSubgroupsPanel.ConfigureButtons();
            SetLabels();
            HideSubgroupsPanel();
        }
        private void SelectAgentType(IEnumerable<object> item)
        {
            var type = item.First() as TagCollection;
            m_agentTypesPanel.Values = m_collections.Select(c => c.name).ToList();
            DisplayTypeSubgroups(type);
        }
        private void DisplayTypeSubgroups(TagCollection type)
        {
            m_typesSubgroupsPanel.SetSourceItems(type.Groups);
            m_typesSubgroupsPanel.style.display = DisplayStyle.Flex;
            m_typesSubgroupsPanel.SetTitle($"{type.name} subgroups");
            m_typesSubgroupsPanel.SetTextFieldValue("");
        }

        private void AddAgentType()
        {
            if (!m_agentTypesPanel.ItemNameIsValid(out string agentType)) return;
            TagCollection item = new(agentType);
            item.Groups.Add("Default");
            m_collections.Add(item);
            m_agentTypesPanel.ListView.RefreshItems();
            m_agentTypesPanel.SetTextFieldValue("");
        }

        private void RemoveAgentType()
        {
            if (!m_agentTypesPanel.CanRemoveSelectedItem()) return;
            var type = m_agentTypesPanel.ListView.selectedItem as TagCollection;
            m_collections.Remove(type);
            TagsManager.RemoveCollection(type);
            m_agentTypesPanel.ListView.RefreshItems();
            HideSubgroupsPanel();
        }

        private void HideSubgroupsPanel()
        {
            m_typesSubgroupsPanel.style.display = DisplayStyle.None;
        }
        private void SetLabels()
        {
            m_agentTypesPanel.SetTitle("Agent Types");
            m_typesSubgroupsPanel.SetTitle("Types Subgroups");

            m_agentTypesPanel.SetItemNameTextFieldLabel("New type:");
            m_typesSubgroupsPanel.SetItemNameTextFieldLabel("New subgroup:");
        }
        private void SaveTagCollections()
        {
            foreach (TagCollection collection in m_collections)
            {
                TagsManager.Save(collection.name, collection);
            }
        }
    }
}