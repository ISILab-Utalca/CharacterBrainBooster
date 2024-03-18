using CBB.Comunication;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace CBB.UI
{
    public class SubgroupBehaviourView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<SubgroupBehaviourView, UxmlTraits> { }
        private Foldout m_rootFoldout;
        private ListView m_agentInstances;
        private DropdownField m_brainDropdown;

        private SubgroupBehaviour m_subgroup;
        public SubgroupBehaviourView()
        {
            var visualTree = Resources.Load<VisualTreeAsset>("Controls/Subgroup Behaviour Details/Subgroup Behaviour View");
            visualTree.CloneTree(this);
            m_rootFoldout = this.Q<Foldout>();
            m_brainDropdown = this.Q<DropdownField>();
            m_agentInstances = this.Q<ListView>();
            m_agentInstances.makeItem = MakeItem;
            m_agentInstances.bindItem = BindItem;
            m_agentInstances.RegisterCallback<MouseUpEvent>(MoveAgents);
            m_brainDropdown.choices = GameData.Brains.Select(brain => brain.name).ToList();
            m_brainDropdown.RegisterValueChangedCallback(OnBrainChanged);
        }

        private void MoveAgents(MouseUpEvent evt)
        {
            if(evt.button == 1)
            {
                var selectedAgents = m_agentInstances.selectedItems.Cast<AgentIdentification>().ToList();
                if(selectedAgents == null || selectedAgents.Count == 0) return;
                var menu = new MoveAgentFloatingPanel();
                
                menu.SetSubgroups(this.userData as string);
                menu.SubgroupSelected += (subgroup) =>
                {
                    foreach (var agent in selectedAgents)
                    {
                        subgroup.AddAgent(agent);
                        m_subgroup.RemoveAgent(agent);
                    }
                    var previousGroup = this.parent.Q<SubgroupBehaviourView>(subgroup.name);
                    previousGroup.m_agentInstances.RefreshItems();
                    m_agentInstances.RefreshItems();
                    TypeBehavioursHandler_ExternalTool.SendTypeBehaviours();
                };
                menu.SetUpPosition(evt.localMousePosition);
                this.Add(menu);
            }
        }

        private void OnBrainChanged(ChangeEvent<string> evt)
        {
            Brain brain = GameData.Brains.Where(brain => brain.name == evt.newValue).FirstOrDefault();
            if (brain == null)
            {
                Debug.LogError("No brain found with the selected name");
                return;
            }
            m_subgroup.brainIdentification = brain.GetBrainIdentification();
        }

        public SubgroupBehaviourView(SubgroupBehaviour subgroup) : this()
        {
            SetData(subgroup);
        }
        private void SetData(SubgroupBehaviour subgroup)
        {
            m_subgroup = subgroup;
            m_rootFoldout.text = subgroup.name;
            m_brainDropdown.value = subgroup.brainIdentification.name;
            m_agentInstances.itemsSource = subgroup.agents;
            this.name = subgroup.name;
        }
        private VisualElement MakeItem()
        {
            return new AgentCard();
        }
        private void BindItem(VisualElement element, int index)
        {
            var item = element as AgentCard;
            item.AgentName.text = m_subgroup.agents[index].name;
            item.AgentID.text = "ID: " + m_subgroup.agents[index].id;
        }
    }
}