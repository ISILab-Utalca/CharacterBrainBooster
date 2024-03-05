using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CBB.UI
{
    public class SubgroupBehaviourDetails : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<SubgroupBehaviourDetails, UxmlTraits> { }
        private Foldout m_rootFoldout;
        private ListView m_agentInstances;
        public SubgroupBehaviourDetails()
        {
            var visualTree = Resources.Load<VisualTreeAsset>("Editor Mode/Subgroup Behaviour Details");
            visualTree.CloneTree(this);
            m_rootFoldout = this.Q<Foldout>();
            m_agentInstances = this.Q<ListView>();
        }
        public SubgroupBehaviourDetails(DataManagement.BrainMap.SubgroupBrain subgroupBrain) : this()
        {
            SetData(subgroupBrain);
        }
        private void SetData(DataManagement.BrainMap.SubgroupBrain subgroupBrain)
        {
            m_rootFoldout.text = subgroupBrain.subgroupName;
        }
    }
}