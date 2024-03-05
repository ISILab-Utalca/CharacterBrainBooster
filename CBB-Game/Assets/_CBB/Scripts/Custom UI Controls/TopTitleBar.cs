using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CBB.ExternalTool
{
    public class TopTitleBar : VisualElement
    {
        #region FACTORY
        public new class UxmlFactory : UxmlFactory<TopTitleBar, UxmlTraits> { }
        #endregion
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlStringAttributeDescription m_WindowTitle = new() { name = "window-title" };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                string text = m_WindowTitle.GetValueFromBag(bag, cc);
                ((TopTitleBar)ve).WindowTitle = text;
                ((TopTitleBar)ve).windowTitleLabel.text = text;
            }
        }
        public string WindowTitle { get; set; } = "Hola";
        public Button CloseButton { get; set; }

        private Label windowTitleLabel;

        public TopTitleBar()
        {
            var visualTree = Resources.Load<VisualTreeAsset>("TopTitleBar");
            visualTree.CloneTree(this);
            windowTitleLabel = this.Q<Label>("window-title");
            CloseButton = this.Q<Button>("close-button");
        }
    }
}
