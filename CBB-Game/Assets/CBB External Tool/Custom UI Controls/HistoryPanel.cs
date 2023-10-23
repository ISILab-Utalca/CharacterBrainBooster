using UnityEngine;
using UnityEngine.UIElements;

namespace CBB.ExternalTool
{
    public class HistoryPanel : VisualElement
    {
        #region FACTORY
        public new class UxmlFactory : UxmlFactory<HistoryPanel, UxmlTraits> { }
        #endregion
        public ShowType myShowField { get; set; }
        private ListView list;
        public enum ShowType
        {
            Decisions,
            SensorEvents,
            Both,
        }
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlEnumAttributeDescription<ShowType> m_myShowField =
                new UxmlEnumAttributeDescription<ShowType> { name = "my-show-field", defaultValue = ShowType.Decisions };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var ate = ve as HistoryPanel;

                ate.myShowField = m_myShowField.GetValueFromBag(bag, cc);
            }
        }
        public HistoryPanel()
        {
            var visualTree = Resources.Load<VisualTreeAsset>("HistoryPanel");
            visualTree.CloneTree(this);
        }
    }
}