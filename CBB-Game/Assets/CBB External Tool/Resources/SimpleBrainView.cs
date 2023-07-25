using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CBB.ExternalTool
{
    public class SimpleBrainView : VisualElement
    {
        #region FACTORY
        public new class UxmlFactory : UxmlFactory<SimpleBrainView, VisualElement.UxmlTraits> { }
        #endregion

        // View
        private Label text;

        public SimpleBrainView()
        {
            var visualTree = Resources.Load<VisualTreeAsset>("SimpleBrainView");
            visualTree.CloneTree(this);

            // PlainText
            this.text = this.Q<Label>();
        }

        public void SetInfo(string brain)
        {
            text.text = brain;
        }
    }
}