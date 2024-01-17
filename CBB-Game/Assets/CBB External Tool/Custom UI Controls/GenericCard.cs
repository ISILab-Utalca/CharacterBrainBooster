using ArtificialIntelligence.Utility;
using Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CBB.ExternalTool
{
    public class GenericCard : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<GenericCard, UxmlTraits> { }

        private Label title;
        private Label subTitle;
        private Button removeButton;
        private DataGeneric data;

        public System.Action<object> DeleteElement { get; set; }
        public GenericCard()
        {
            var vt = Resources.Load<VisualTreeAsset>("Editor Mode/Generic Card");
            vt.CloneTree(this);
            title = this.Q<Label>("title");
            subTitle = this.Q<Label>("subtitle");
            removeButton = this.Q<Button>("remove-button");
            removeButton.clickable.clicked += () =>
            {
                DeleteElement?.Invoke(data);
                this.RemoveFromHierarchy();
            };
        }
        public GenericCard(DataGeneric data, Color subtitleColor) : this()
        {
            this.data = data;
            SetTitle(HelperFunctions.RemoveNamespace(data.ClassType.Name));
            SetSubtitleText(data.GetDataType().ToString());
            SetSubtitleColor(subtitleColor);
        }
        public void SetTitle(string title)
        {
            this.title.text = title;
        }
        public void SetSubtitleText(string subTitle)
        {
            this.subTitle.text = subTitle;
        }
        public void SetSubtitleColor(Color color)
        {
            this.subTitle.style.color = color;
        }
    }
}