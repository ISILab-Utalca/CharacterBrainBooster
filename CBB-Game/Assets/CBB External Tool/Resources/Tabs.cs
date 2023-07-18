using System.Collections;
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
        private ListView List;

        // Info
        private List<string> target;

        public Tabs()
        {
            var visualTree = Resources.Load<VisualTreeAsset>("Tabs");
            visualTree.CloneTree(this);

            // History list
            this.List = this.Q<ListView>();
            List.bindItem += BindItem;
            List.makeItem += MakeItem;
            List.itemsChosen += OnItemChosen;
            List.selectionChanged += OnSelectionChange;
        }


        private VisualElement MakeItem() // hacer que esto sea un solo viewElement (!!!)
        {
            var content = new VisualElement();
            var textLabel = new Label();
            textLabel.name = "text";

            return content;
        }

        private void BindItem(VisualElement element, int index)
        {
            var nameLabel = element.Q<Label>("text");
            nameLabel.text = target[index];
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