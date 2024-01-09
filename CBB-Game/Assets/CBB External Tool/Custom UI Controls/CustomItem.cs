using CBB.ExternalTool;
using UnityEngine.UIElements;
using UnityEngine;
namespace CBB.UI

{
    public class CustomItem : VisualElement, IDataItem
    {
        public new class UxmlFactory : UxmlFactory<CustomItem, UxmlTraits> { }

        public Button ActionButton { get; set; }
        public Label ItemName { get; set; }

        public CustomItem()
        {
            var visualTree = Resources.Load<VisualTreeAsset>("Editor Mode/Button Item");

            visualTree.CloneTree(this);

            ActionButton = this.Q<Button>("action-button");
            ItemName = this.Q<Label>("item-name");
        }
        public string GetItemName() => "+";
        public object GetInstance() => this;
        public void HideActionButton()
        {
            if(ActionButton != null)
            {
                ActionButton.style.display = DisplayStyle.None;
            }
        }

    }

}
