using UnityEngine;
using UnityEngine.UIElements;

namespace CBB.UI
{
    public class SubgroupListItem : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<SubgroupListItem> { }
        public Label Label { get; private set; }
        public SubgroupListItem()
        {
            var visualTree = Resources.Load<VisualTreeAsset>("Controls/Subgroup List Item/Subgroup List Item");
            visualTree.CloneTree(this);
            Label = this.Q<Label>();
            this.RegisterCallback<MouseDownEvent>(evt => evt.StopPropagation());
        }
    }
}