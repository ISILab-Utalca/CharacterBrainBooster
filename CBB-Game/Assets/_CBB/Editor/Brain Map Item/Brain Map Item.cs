using CBB.DataManagement;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace CBB.InternalTool
{
    public class BrainMapItem : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<BrainMapItem> { }
        private Label m_subgroupTitle;
        private DropdownField m_brainsDropdown;
        private BrainMap.SubgroupBrain m_subgroupBehaviour;
        public BrainMapItem()
        {
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/_CBB/Editor/Brain Map Item/Brain Map Item.uxml");
            visualTree.CloneTree(this);
            GetReferences();
            LoadBrains();
            RegisterDropdownCallback();
        }
        private void GetReferences()
        {
            m_subgroupTitle = this.Q<Label>();
            m_brainsDropdown = this.Q<DropdownField>();
        }
        private void LoadBrains()
        {
            var brains = BrainDataLoader.GetAllBrains();
            var brainNames = new List<string>();
            foreach (var brain in brains)
            {
                brainNames.Add(brain.name);
            }
            m_brainsDropdown.choices = brainNames;
        }
        private void RegisterDropdownCallback()
        {
            m_brainsDropdown.RegisterValueChangedCallback(evt =>
            {
                if (string.IsNullOrEmpty(evt.newValue)) return;
                var brain = BrainDataLoader.GetBrainByName(evt.newValue);
                if (brain == null) return;
                m_subgroupBehaviour.brainID = brain.id;
            });
        }
        internal void SetData(BrainMap.SubgroupBrain subgroupBehaviour)
        {
            m_subgroupBehaviour = subgroupBehaviour;
            m_subgroupTitle.text = subgroupBehaviour.subgroupName;
            if (string.IsNullOrEmpty(subgroupBehaviour.brainID))
            {
                m_brainsDropdown.value = "Default";
                return;
            }
            var brain = BrainDataLoader.GetBrainByID(subgroupBehaviour.brainID);
            m_brainsDropdown.value = brain.name;
        }
    }
}
