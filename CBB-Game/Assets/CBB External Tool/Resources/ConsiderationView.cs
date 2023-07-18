using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CBB.ExternalTool
{
    public class ConsiderationView : VisualElement
    {
        #region FACTORY
        public new class UxmlFactory : UxmlFactory<ConsiderationView, VisualElement.UxmlTraits> { }
        #endregion

        // View
        private Chart chart;

        public ConsiderationView()
        {
            var visualTree = Resources.Load<VisualTreeAsset>("ConsiderationView");
            visualTree.CloneTree(this);

            // Chart
            this.chart = this.Q<Chart>();
            
        }
    }
}