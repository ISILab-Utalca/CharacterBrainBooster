using CBB.Comunication;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CBB.UI
{
    public class MoveAgentFloatingPanel : FloatingPanelBase
    {
        public new class UxmlFactory : UxmlFactory<MoveAgentFloatingPanel, UxmlTraits> { }
        private ListView m_subgroupsListView;
        private List<SubgroupBehaviour> m_subgroupBehaviours;
        public Action<SubgroupBehaviour> SubgroupSelected { get; set;}
        public MoveAgentFloatingPanel() : base()
        {
            var visualTree = Resources.Load<VisualTreeAsset>("Controls/Move Agent Floating Panel/Move Agent Floating Panel");
            visualTree.CloneTree(this);
            m_subgroupsListView = this.Q<ListView>();
            m_subgroupsListView.makeItem = MakeItem;
            m_subgroupsListView.bindItem = BindItem;
            m_subgroupsListView.selectionChanged += ElementClicked;
        }

        private void ElementClicked(IEnumerable<object> enumerable)
        {
            SubgroupSelected?.Invoke(m_subgroupBehaviours[m_subgroupsListView.selectedIndex]);
            Close();
        }

        public void SetSubgroups(string agentType)
        {
            m_subgroupBehaviours = GameData.TypeBehaviours.Find(typeBehaviour => typeBehaviour.agentType == agentType).subgroups;
            m_subgroupsListView.itemsSource = m_subgroupBehaviours;
            m_subgroupsListView.RefreshItems();
        }

        private void BindItem(VisualElement element, int index)
        {
            (element as SubgroupListItem).Label.text = m_subgroupBehaviours[index].name;
        }

        private VisualElement MakeItem()
        {
            return new SubgroupListItem();
        }
    }
}