using CBB.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace CBB.ExternalTool
{
    public class AgentsPanel : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<AgentsPanel, UxmlTraits> { }
        
        public AgentsPanel()
        {
            var visualTree = Resources.Load<VisualTreeAsset>("AgentsPanel");
            visualTree.CloneTree(this);
        }
    }
}