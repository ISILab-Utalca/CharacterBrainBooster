using CBB.DataManagement;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace CBB.InternalTool
{
    [CustomEditor(typeof(BehaviourLoader))]
    public class BehaviourLoader_Inspector : Editor
    {
        public VisualTreeAsset m_VisualTreeAsset = default;
        private VisualElement m_root;
        private DropdownField m_agentTypeDropdown;
        private DropdownField m_typeSubgroupsDropdown;
        private List<BrainMap> m_brainMaps = new();
        private List<string> m_agentTypeNames = new();
        public override VisualElement CreateInspectorGUI()
        {
            m_root = new VisualElement();
            m_VisualTreeAsset.CloneTree(m_root);
            LoadAgentTypes();
            SetReferences();
            InitializeDropdowns();
            SetDropdownBehaviours();
            return m_root;
        }
        private void LoadAgentTypes()
        {
            m_brainMaps = BrainMapsManager.GetAllBrainMaps();
            m_agentTypeNames.Clear();
            foreach (BrainMap collection in m_brainMaps)
            {
                m_agentTypeNames.Add(collection.name);
            }
        }
        private void SetReferences()
        {
            m_agentTypeDropdown = m_root.Q<DropdownField>("agent-type-dropdown");
            m_typeSubgroupsDropdown = m_root.Q<DropdownField>("type-subgroups-dropdown");
        }
        private void InitializeDropdowns()
        {
            m_agentTypeDropdown.choices = m_agentTypeNames;
            SetSubgroups(m_agentTypeDropdown.value);

        }
        private void SetSubgroups(string selectedType)
        {
            List<string> subgroup = GetSelectedTypeSubgroups(selectedType);
            if (subgroup == null) return;
            m_typeSubgroupsDropdown.choices = subgroup;
        }
        private List<string> GetSelectedTypeSubgroups(string selectedType)
        {
            if (!string.IsNullOrEmpty(selectedType))
            {
                foreach (var collection in m_brainMaps)
                {
                    if (collection.name == selectedType)
                    {
                        var subgroupsNames = new List<string>();
                        foreach (var subgroup in collection.SubgroupsBrains)
                        {
                            subgroupsNames.Add(subgroup.subgroupName);
                        }
                        return subgroupsNames;
                    }
                }
            }

            return null;
        }
        private void SetDropdownBehaviours()
        {
            m_agentTypeDropdown.RegisterValueChangedCallback(OnSelectedAgentType);
        }

        private void OnSelectedAgentType(ChangeEvent<string> evt)
        {
            SetSubgroups(evt.newValue);
        }
    } 
}
