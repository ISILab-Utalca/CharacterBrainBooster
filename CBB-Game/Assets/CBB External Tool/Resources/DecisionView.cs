using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CBB.ExternalTool
{
    public class DecisionView : VisualElement
    {
        #region FACTORY
        public new class UxmlFactory : UxmlFactory<DecisionView, UxmlTraits> { }
        #endregion

        // View
        private Chart chart;
        private VisualElement content;
        private List<Consideration> considerations = new List<Consideration>();

        public DecisionView()
        {
            var visualTree = Resources.Load<VisualTreeAsset>("DecisionView");
            visualTree.CloneTree(this);

            // Chart
            this.chart = this.Q<Chart>();

            // Consideration Content
            this.content = this.Q<VisualElement>("ConsiderationContent");
        }

    }
}