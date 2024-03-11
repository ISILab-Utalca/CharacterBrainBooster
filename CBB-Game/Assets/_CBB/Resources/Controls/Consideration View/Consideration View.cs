using CBB.Lib;
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
        private Foldout foldout;
        private Label input;
        private Label utility;
        public ConsiderationView()
        {
            var visualTree = Resources.Load<VisualTreeAsset>("Controls/Consideration View/Consideration View");
            visualTree.CloneTree(this);

            // Chart
            chart = this.Q<Chart>();
            foldout = this.Q<Foldout>();
            input = this.Q<Label>("input");
            utility = this.Q<Label>("utility");
        }
        public void ShowConsideration(ConsiderationData consideration)
        {
            chart.SetCurve(consideration.Curve, consideration.InputValue, true);
            foldout.text = consideration.ConsiderationName;
            input.text = $"{consideration.EvaluatedVariableName}: {consideration.InputValue}";
            utility.text = $"Utility: {consideration.UtilityValue}";
        }
    }
}