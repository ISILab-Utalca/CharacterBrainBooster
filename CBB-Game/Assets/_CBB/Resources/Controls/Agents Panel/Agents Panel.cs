using UnityEngine;
using UnityEngine.UIElements;
namespace CBB.ExternalTool
{
    public class AgentsPanel : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<AgentsPanel, UxmlTraits> { }
        
        public AgentsPanel()
        {
            var visualTree = Resources.Load<VisualTreeAsset>("Controls/Agents Panel/Agents Panel");
            visualTree.CloneTree(this);
            this.style.flexGrow = 1;
        }
    }
}