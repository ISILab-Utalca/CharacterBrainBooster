using CBB.Lib;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CBB.ExternalTool
{
    public class AgentInfo : VisualElement
    {
        #region FACTORY
        public new class UxmlFactory : UxmlFactory<AccionInfo, UxmlTraits> { }
        #endregion

        public AgentInfo()
        {
            var visualTree = Resources.Load<VisualTreeAsset>("AgentInfo");
            visualTree.CloneTree(this);
        }
    }
}