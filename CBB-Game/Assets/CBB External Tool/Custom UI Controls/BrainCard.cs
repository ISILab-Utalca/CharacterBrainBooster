using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CBB.ExternalTool
{
    public class BrainCard : VisualElement
    {
        #region CONTROL FACTORY
        public new class UxmlFactory : UxmlFactory<BrainCard, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlStringAttributeDescription m_WindowTitle = new() { name = "" };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                //string text = m_WindowTitle.GetValueFromBag(bag, cc);
                //((BrainCard)ve).WindowTitle = text;
                //((BrainCard)ve).windowTitleLabel.text = text;
            }
        }
        #endregion

        public Label brainName;
        public Label brainID;
        public BrainCard()
        {
            var visualTree = Resources.Load<VisualTreeAsset>("Editor Mode/Brain Card");
            
            visualTree.CloneTree(this);
            brainName = this.Q<Label>("brain-name");
            brainID = this.Q<Label>("brain-id");
        }
    }
}
