using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CBB.ExternalTool
{
    public class BindingsPanel : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<BindingsPanel, UxmlTraits> { }

        private TreeView m_bindingsTreeView;
        public BindingsPanel()
        {
            var visualTree = Resources.Load<VisualTreeAsset>("Editor Mode/Bindings Panel");
            visualTree.CloneTree(this);
            m_bindingsTreeView = this.Q<TreeView>();
        }
    }
}
