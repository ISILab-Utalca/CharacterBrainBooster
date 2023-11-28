using CBB.Lib;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CBB.ExternalTool
{
    public class ActionInfo : VisualElement
    {
        #region FACTORY
        public new class UxmlFactory : UxmlFactory<ActionInfo, UxmlTraits> { }
        #endregion
        #region PROPERTIES

        public Label ActionName { get; set; }
        public Label TargetName { get; set; }
        public Label ActionScore { get; set; }
        public Label TimeStamp { get; set; }
        public Label ActionID { get; set; }
        #endregion
        public ActionInfo()
        {
            var visualTree = Resources.Load<VisualTreeAsset>("ActionInfo");
            visualTree.CloneTree(this);

            ActionName = this.Q<Label>("action-name");
            TargetName = this.Q<Label>("target-name");
            ActionScore = this.Q<Label>("action-score");
            TimeStamp = this.Q<Label>("timestamp");
            ActionID = this.Q<Label>("action-agent_ID");

        }
    }
}
