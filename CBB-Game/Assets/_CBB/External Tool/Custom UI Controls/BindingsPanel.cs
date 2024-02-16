using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CBB.ExternalTool
{
    public class BindingsPanel : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<BindingsPanel, UxmlTraits> { }

        private TreeView m_bindingsTreeView;
        private DropdownField m_agentTypes;

        public BindingsPanel()
        {
            var visualTree = Resources.Load<VisualTreeAsset>("Editor Mode/Bindings Panel");
            visualTree.CloneTree(this);
            m_bindingsTreeView = this.Q<TreeView>();
            m_agentTypes = this.Q<DropdownField>();
        }
        public void SetAgentTypes(List<string> agentTypes)
        {
            m_agentTypes.Clear();
            m_agentTypes.choices = agentTypes;
        }
    }
}
