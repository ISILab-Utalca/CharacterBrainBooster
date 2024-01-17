using ArtificialIntelligence.Utility;
using Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CBB.ExternalTool
{
    public class GenericCard : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<GenericCard, UxmlTraits> { }

        public System.Action<object> DeleteElement { get; set; }
        public Label Title { get; set; }
        public Label SubTitle { get; }
        public Button RemoveButton { get; set; }
        public object Data { get; set; }

        public GenericCard()
        {
            var vt = Resources.Load<VisualTreeAsset>("Editor Mode/Generic Card");
            vt.CloneTree(this);
            Title = this.Q<Label>("title");
            SubTitle = this.Q<Label>("subtitle");
            RemoveButton = this.Q<Button>("remove-button");
            RemoveButton.clicked += () => DeleteElement?.Invoke(Data);
        }
        public GenericCard(object data) : this()
        {
            this.Data = data;
        }
        public void SetTitle(string title)
        {
            this.Title.text = title;
        }
        public void SetSubtitleText(string subTitle)
        {
            this.SubTitle.text = subTitle;
        }
        public void SetSubtitleColor(Color color)
        {
            this.SubTitle.style.color = color;
        }
    }
}