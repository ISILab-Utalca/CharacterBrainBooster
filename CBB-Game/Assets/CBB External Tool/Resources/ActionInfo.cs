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
        #endregion
        public ActionInfo()
        {
            var visualTree = Resources.Load<VisualTreeAsset>("ActionInfo");
            visualTree.CloneTree(this);

            ActionName = this.Q<Label>("ActionName");
            TargetName = this.Q<Label>("TargetName");
            ActionScore = this.Q<Label>("ActionScore");
        }
    }
}
