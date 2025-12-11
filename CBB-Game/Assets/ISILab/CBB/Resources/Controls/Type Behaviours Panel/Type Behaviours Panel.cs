using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CBB.UI
{
    public class TypeBehavioursPanel : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<TypeBehavioursPanel, UxmlTraits> { }

        private VisualElement m_contentContainer;

        public DropdownField AgentTypes { get; set; }

        public TypeBehavioursPanel()
        {
            var visualTree = Resources.Load<VisualTreeAsset>("Controls/Type Behaviours Panel/Type Behaviours Panel");
            visualTree.CloneTree(this);
            AgentTypes = this.Q<DropdownField>();
            m_contentContainer = this.Q<VisualElement>("brain-maps-container");
        }
        public void SetAgentTypes(List<string> agentTypes)
        {
            AgentTypes.Clear();
            AgentTypes.choices = agentTypes;
        }
        public void ClearContent()
        {
            m_contentContainer.Clear();
        }
        public void AddContent(SubgroupBehaviourView brainMap)
        {
            m_contentContainer.Add(brainMap);
        }
    }
}
