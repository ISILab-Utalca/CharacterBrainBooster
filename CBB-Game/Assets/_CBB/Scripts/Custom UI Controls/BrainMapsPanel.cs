using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CBB.UI
{
    public class BrainMapsPanel : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<BrainMapsPanel, UxmlTraits> { }

        private DropdownField m_agentTypes;
        private VisualElement m_brainMapsContainer;
        public BrainMapsPanel()
        {
            var visualTree = Resources.Load<VisualTreeAsset>("Editor Mode/Brain Maps Panel");
            visualTree.CloneTree(this);
            m_agentTypes = this.Q<DropdownField>();
            m_brainMapsContainer = this.Q<VisualElement>("brain-maps-container");
        }
        public void SetAgentTypes(List<string> agentTypes)
        {
            m_agentTypes.Clear();
            m_agentTypes.choices = agentTypes;
        }
        public void ClearBrainMaps()
        {
            m_brainMapsContainer.Clear();
        }
        public void AddBrainMap(SubgroupBehaviourDetails brainMap)
        {
            m_brainMapsContainer.Add(brainMap);
        }
    }
}
