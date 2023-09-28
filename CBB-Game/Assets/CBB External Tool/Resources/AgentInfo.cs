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
        public new class UxmlFactory : UxmlFactory<AgentInfo, UxmlTraits> { }
        #endregion
        #region PROPERTIES
        public Label AgentName { get; set; }
        public Label AgentID { get; set; }
        #endregion
        public AgentInfo()
        {
            var visualTree = Resources.Load<VisualTreeAsset>("AgentInfo");
            visualTree.CloneTree(this);

            AgentName = this.Q<Label>("agent-name");
            AgentID = this.Q<Label>("agent-id");
        }
    }
}