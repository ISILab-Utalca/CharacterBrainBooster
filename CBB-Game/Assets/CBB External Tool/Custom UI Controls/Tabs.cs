using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CBB.ExternalTool
{
    public class Tabs : VisualElement
    {
        #region FACTORY
        public new class UxmlFactory : UxmlFactory<Tabs, VisualElement.UxmlTraits> { }
        #endregion

        // View
        private ListView list;

        // Info
        private List<(string, Action)> target = new List<(string, Action)>();

        public Action OnSelectOption;

        public Tabs()
        {
            var visualTree = Resources.Load<VisualTreeAsset>("Tabs");
            visualTree.CloneTree(this);

            // History list
            this.list = this.Q<ListView>();
            list.bindItem += BindItem;
            list.makeItem += MakeItem;
            list.itemsChosen += OnItemChosen;
            list.selectionChanged += OnSelectionChange;
        }

        public new void Clear()
        {
            //base.Clear();

            target.Clear();
            list.Clear();
        }

        public void AddTabs(string value, Action action)
        {
            var tab = new Button();
            tab.text = value;

            tab.clicked += action;
            tab.clicked += OnSelectOption;
        }

        private VisualElement MakeItem() // hacer que esto sea un solo viewElement (!!!) 
        {
            var content = new Button();
            content.name = "text";

            return content;
        }

        private void BindItem(VisualElement element, int index)
        {
            var button = element.Q<Button>("text");
            button.text = target[index].Item1;
            //button.clicked += target[index].Item2;
        }

        public void OnSelectionChange(IEnumerable<object> objs)
        {
            Debug.Log("OnSelectionChange");
        }

        public void OnItemChosen(IEnumerable<object> objs)
        {
            Debug.Log("OnItemChosen");
        }
    }
}