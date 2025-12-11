using UnityEngine;
using UnityEngine.UIElements;

namespace CBB.UI
{
    public class AgentCard : VisualElement
    {
        #region FACTORY
        public new class UxmlFactory : UxmlFactory<AgentCard, UxmlTraits> { }
        #endregion
        #region PROPERTIES
        public Label AgentName { get; set; }
        public Label AgentID { get; set; }
        #endregion
        public AgentCard()
        {
            var visualTree = Resources.Load<VisualTreeAsset>("Controls/Agent Card/Agent Card");
            visualTree.CloneTree(this);

            AgentName = this.Q<Label>("agent-name");
            AgentID = this.Q<Label>("agent-id");
        }
    }
}