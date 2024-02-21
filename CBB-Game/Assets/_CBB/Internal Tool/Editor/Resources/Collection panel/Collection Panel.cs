using CBB.DataManagement;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CBB.InternalTool
{
    public class CollectionPanel : VisualElement
    {
        #region Fields
        private Label m_title;
        private ListView m_listView;
        private Button m_addItemButton;
        private Button m_removeItemButton;
        private TextField m_itemNameTextField;
        private List<string> m_values = new();
        #endregion

        #region Properties
        public Button AddItemButton { get => m_addItemButton; }
        public Button RemoveItemButton { get => m_removeItemButton; }
        public ListView ListView { get => m_listView; }
        public List<string> Values { get => m_values; set => m_values = value; }
        #endregion

        public CollectionPanel()
        {
            var visualTree = Resources.Load<VisualTreeAsset>("Collection panel/Collection panel");
            visualTree.CloneTree(this);

            GetLocalReferences();
            ConfigureListView();
            SetTextFieldValue("Default");
        }
        private void GetLocalReferences()
        {
            m_listView = this.Q<ListView>();
            m_addItemButton = this.Q<Button>("add-item-button");
            m_removeItemButton = this.Q<Button>("remove-item-button");
            m_title = this.Q<Label>("collection-title");
            m_itemNameTextField = this.Q<TextField>("item-name-text-field");
        }
        private void ConfigureListView()
        {
            m_listView.makeItem = MakeItem;
            m_listView.itemsSource = m_values;
            m_listView.bindItem = (e, i) => (e as Label).text = m_values[i];
        }
        public void ConfigureButtons()
        {
            // Note: not called automatically to allow setting
            // custom behaviours from other scripts to these elements
            m_addItemButton.clicked += AddItem;
            m_removeItemButton.clicked += RemoveItem;
            m_itemNameTextField.RegisterCallback(OnTextFieldKeyDown(AddItem));
        }

        private EventCallback<KeyDownEvent> OnTextFieldKeyDown(Action callback)
        {
            return (KeyDownEvent evt) =>
            {
                if (evt.keyCode == KeyCode.Return)
                {
                    callback();
                }
            };
            
        }
        public void RegisterCallbackOnTextFieldEnterPressed(Action callback)
        {
            m_itemNameTextField.RegisterCallback(OnTextFieldKeyDown(callback));
        }
        private void AddItem()
        {
            if (m_values == null) return;
            if (!ItemNameIsValid(out string itemName)) return;
            m_values.Add(itemName);
            m_listView.RefreshItems();
            SetTextFieldValue("");
        }
        public bool ItemNameIsValid(out string itemName)
        {
            itemName = m_itemNameTextField.value.Trim();
            if (string.IsNullOrEmpty(itemName))
            {
                Debug.LogError("Name cannot be empty");
                return false;
            }
            if (m_values.Contains(itemName))
            {
                Debug.LogError("Name already exists");
                return false;
            }
            return true;
        }
        private void RemoveItem()
        {
            if (!CanRemoveSelectedItem()) return;
            m_values.Remove(m_listView.selectedItem as string);
            m_listView.RefreshItems();
        }
        public bool CanRemoveSelectedItem()
        {
            if (m_values == null) return false;
            if (m_values.Count == 0) return false;
            if (m_listView == null) return false;
            if (m_listView.selectedIndex == -1) return false;
            return true;
        }

        #region Setters
        public void SetTitle(string title) => m_title.text = title;
        public void SetItemNameTextFieldLabel(string label) => m_itemNameTextField.label = label;
        public void SetSourceItems(List<string> values)
        {
            m_values = values;
            m_listView.itemsSource = m_values;
            m_listView.RefreshItems();
        }
        public void SetTextFieldValue(string value) => m_itemNameTextField.value = value;
        #endregion

        private VisualElement MakeItem()
        {
            return new Label();
        }

        public new class UxmlFactory : UxmlFactory<CollectionPanel> { }
    }
}