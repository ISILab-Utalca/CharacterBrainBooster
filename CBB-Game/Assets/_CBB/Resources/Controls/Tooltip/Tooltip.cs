using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CBB.UI
{
    public class Tooltip : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<Tooltip, UxmlTraits> { }
        public Label Label { get; private set; }
        private VisualElement _target;
        public Tooltip()
        {
            var visualTree = Resources.Load<VisualTreeAsset>("Controls/Tooltip/Tooltip");
            visualTree.CloneTree(this);
            Label = this.Q<Label>();
        }
    }
}