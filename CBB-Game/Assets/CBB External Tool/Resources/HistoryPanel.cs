using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CBB.ExternalTool
{
    public class HistoryPanel : VisualElement
    {
        #region FACTORY
        public new class UxmlFactory : UxmlFactory<HistoryPanel, VisualElement.UxmlTraits> { }
        #endregion

        public enum ShowType
        {
            Decisions,
            SensorEvents,
            Both
        }

        // View
        private ListView List;
        private EnumField showField;
        private Tabs tabs;
        private ScrollView scroll;

        // Info
        private List<string> target;
        private ShowType showType = ShowType.Both;

        public HistoryPanel()
        {
            var visualTree = Resources.Load<VisualTreeAsset>("HistoryPanel");
            visualTree.CloneTree(this);

            // History list
            this.List = this.Q<ListView>();
            List.bindItem += BindItem;
            List.makeItem += MakeItem;
            List.itemsChosen += OnItemChosen;
            List.selectionChanged += OnSelectionChange;

            // Show dropdown
            this.showField = this.Q<EnumField>();
            showField.RegisterCallback<ChangeEvent<Enum>>((evt) => OnChangeShowType(evt));
            // cuando este cambie actualiza la lista. y se deseleciona lo selecionado


        }

        private VisualElement MakeItem() // hacer que esto sea un solo viewElement (!!!)
        {
            var content = new VisualElement();
            var nameLabel = new Label();
            nameLabel.name = "name";
            if (showType == ShowType.Both || showType == ShowType.Decisions) // Decision
            {
                /*
                var idLabel = new Label();
                idLabel.name = "id";
                content.Add(idLabel);
                content.Add(nameLabel);
                */
            }
            if (showType == ShowType.Both || showType == ShowType.SensorEvents) // Sensor events
            {
                // Impleentar (!!!)
            }

            return content;
        }

        private void BindItem(VisualElement element, int index)
        {
            var nameLabel = element.Q<Label>("name");
            nameLabel.text = target[index];

            if (showType == ShowType.Both || showType == ShowType.Decisions) // Decision
            {
                /*
                var idLabel = element.Q<Label>("id");
                idLabel.text = index.ToString(); // sacar el indice del agente y no de la propia lista (!!!)
                */
            }
            if (showType == ShowType.Both || showType == ShowType.SensorEvents) // Sensor events
            {
                // Impleentar (!!!)
            }
        }

        private void OnSelectionChange(IEnumerable<object> objs)
        {
            Debug.Log("OnSelectionChange");
        }

        private void OnItemChosen(IEnumerable<object> objs)
        {
            Debug.Log("OnItemChosen");
        }

        private void OnChangeShowType(ChangeEvent<Enum> evt)
        {
            var value = evt.newValue;
            Debug.Log(value.ToString());
        }
    }
}