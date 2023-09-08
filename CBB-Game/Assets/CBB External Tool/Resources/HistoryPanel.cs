using CBB.Lib;
using System;
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
        private ListView list;
        private EnumField showField;
        private Tabs tabs;
        private ScrollView scroll;

        // Info
        private List<DecisionPackage> target;
        private ShowType showType = ShowType.Both;

        public HistoryPanel()
        {
            var visualTree = Resources.Load<VisualTreeAsset>("HistoryPanel");
            visualTree.CloneTree(this);

            // History list
            this.list = this.Q<ListView>();
            list.bindItem += BindItem;
            list.makeItem += MakeItem;
            list.itemsChosen += OnItemChosen;
            list.selectionChanged += OnSelectionChange;

            // Show dropdown
            this.showField = this.Q<EnumField>();
            showField.RegisterCallback<ChangeEvent<Enum>>((evt) => OnChangeShowType(evt));
            // cuando este cambie actualiza la lista. y se deseleciona lo selecionado

            // Tabs
            this.tabs = this.Q<Tabs>();
            tabs.OnSelectOption += OnSelectionOption;
        }

        private void OnSelectionOption()
        {

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
            nameLabel.text = target[index].GetType().ToString();

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

        public void SetInfo(List<DecisionPackage> history)
        {
            this.target = history;
        }

        public void Actualize()
        {
            list.Clear();
            list.RefreshItems();
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