using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CBB.UI
{
    public class MoveAgentFloatingPanel : FloatingPanelBase
    {
        public new class UxmlFactory : UxmlFactory<MoveAgentFloatingPanel, UxmlTraits> { }
        private Foldout m_rootFoldout;
        private ListView m_agentInstances;
        public MoveAgentFloatingPanel() : base()
        {
            var visualTree = Resources.Load<VisualTreeAsset>("Controls/Move Agent Floating Panel/Move Agent Floating Panel");
            visualTree.CloneTree(this);
            m_rootFoldout = this.Q<Foldout>();
            m_agentInstances = this.Q<ListView>();
            m_agentInstances.makeItem = MakeItem;
            m_agentInstances.bindItem = BindItem;
            m_agentInstances.RegisterCallback<MouseUpEvent>(MoveAgents);
        }

        private void MoveAgents(MouseUpEvent evt)
        {
            throw new NotImplementedException();
        }

        private void BindItem(VisualElement element, int arg2)
        {
            throw new NotImplementedException();
        }

        private VisualElement MakeItem()
        {
            throw new NotImplementedException();
        }
    }
}