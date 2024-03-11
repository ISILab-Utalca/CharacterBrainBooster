using CBB.Comunication;
using CBB.ExternalTool;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace CBB.UI
{
    public class SubgroupBehaviourDetails : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<SubgroupBehaviourDetails, UxmlTraits> { }
        private Foldout m_rootFoldout;
        private ListView m_agentInstances;
        private DropdownField m_brainDropdown;

        private SubgroupBehaviour m_subgroup;
        public SubgroupBehaviourDetails()
        {
            var visualTree = Resources.Load<VisualTreeAsset>("Editor Mode/Subgroup Behaviour Details");
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
                var selectedAgent = m_agentInstances.selectedItems as System.Collections.Generic.IEnumerable<AgentIdentification>;
                if (selectedAgent == null) return;
                var menu = new MoveAgentFloatingPanel();
                menu.SetUpPosition(evt.mousePosition);
                this.Add(menu);
                //menu.AddItem(new GUIContent("Move to"), false, () => MoveAgentTo(selectedAgent));
                //menu.ShowAsContext();
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

        public SubgroupBehaviourDetails(SubgroupBehaviour subgroup) : this()
        {
            SetData(subgroup);
        }
        private void SetData(SubgroupBehaviour subgroup)
        {
            m_subgroup = subgroup;
            m_rootFoldout.text = subgroup.name;
            m_brainDropdown.value = subgroup.brainIdentification.name;
            m_agentInstances.itemsSource = subgroup.agents;
        }
        private VisualElement MakeItem()
        {
            return new AgentInfo();
        }
        private void BindItem(VisualElement element, int index)
        {
            var item = element as AgentInfo;
            item.AgentName.text = m_subgroup.agents[index].name;
            item.AgentID.text = "ID: " + m_subgroup.agents[index].id;
        }
    }
}