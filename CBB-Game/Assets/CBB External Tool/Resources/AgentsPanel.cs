using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CBB.ExternalTool
{
    public class AgentsPanel : VisualElement
    {
        #region FACTORY
        public new class UxmlFactory : UxmlFactory<AgentsPanel, VisualElement.UxmlTraits> { }
        #endregion

        // View
        private ListView list;

        private List<string> target;

        public Action<IEnumerable<object>> ItemChosen;
        public Action<IEnumerable<object>> SelectionChange;

        public AgentsPanel()
        {
            var visualTree = Resources.Load<VisualTreeAsset>("AgentsPanel");
            visualTree.CloneTree(this);

            this.list = this.Q<ListView>();
            list.bindItem += BindItem;
            list.makeItem += MakeItem;
            list.itemsChosen += OnItemChosen; // cambiar a evento (!)
            list.selectionChanged += OnSelectionChange; // cambiar a evento (!)
        }

        private VisualElement MakeItem() // hacer que esto sea un solo viewElement (!!!)
        {
            var nameLabel = new Label();
            nameLabel.name = "name";

            var idLabel = new Label();
            idLabel.name = "id";

            var content = new VisualElement();
            content.Add(idLabel);
            content.Add(nameLabel);

            return content;
        }

        private void BindItem(VisualElement element, int index)
        {
            var nameLabel = element.Q<Label>("name");
            nameLabel.text = target[index];

            var idLabel = element.Q<Label>("id");
            idLabel.text = index.ToString(); // sacar el indice del agente y no de la propia lista (!!!)
        }

        public void OnSelectionChange(IEnumerable<object> objs)
        {
            Debug.Log("OnSelectionChange");
            SelectionChange?.Invoke(objs);
        }

        public void OnItemChosen(IEnumerable<object> objs)
        {
            Debug.Log("OnItemChosen");
            ItemChosen?.Invoke(objs);
        }
    }
}