using UnityEngine;
using UnityEngine.UIElements;

namespace CBB.ExternalTool
{
    public class GenericCard : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<GenericCard, UxmlTraits> { }

        private Label title;
        private Label subtitle;
        public GenericCard()
        {
            var vt = Resources.Load<VisualTreeAsset>("Editor Mode/Generic Card");
            vt.CloneTree(this);
            title = this.Q<Label>("title");
            subtitle = this.Q<Label>("subtitle");
        }
        public void SetTitle(string title)
        {
            this.title.text = title;
        }
        public void SetSubtitle(string subtitle)
        {
            this.subtitle.text = subtitle;
        }
    }
}