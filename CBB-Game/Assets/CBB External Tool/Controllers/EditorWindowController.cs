using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;


namespace CBB.ExternalTool
{
    public class EditorWindowController : MonoBehaviour
    {
        // UI
        private TreeView brainTree;
        private Button reloadBrainsButton;
        // Logic
        [SerializeField]
        private BrainDataManager brainDataManager;

        private void Awake()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            brainTree = root.Q<TreeView>("brain-tree");
            reloadBrainsButton = root.Q<Button>("reload-brains-button");
            reloadBrainsButton.clicked += LoadBrains;
            // Set TreeView.makeItem to initialize each node in the tree.
            brainTree.makeItem = MakeItem;

            // Set TreeView.bindItem to bind an initialized node to a data item.
            brainTree.bindItem = BindItem;
        }
        /// <summary>
        /// Retrieve all the brains existing in the game
        /// </summary>
        public void LoadBrains()
        {
            // Display the brains in the tree view
            //brainTree.Clear();
            // Call brainTree.SetRootItems() to populate the data in the tree.
            brainTree.SetRootItems(brainDataManager.TreeRoots);
            brainTree.RefreshItems();
        }
        private void OnEnable()
        { 
            LoadBrains();
        }
        private VisualElement MakeItem()
        {
            return new Label();
        }
        private void BindItem(VisualElement element, int index)
        {
            var item = element as Label;
            var itemName = brainTree.GetItemDataForIndex<IDataItem>(index).GetItemName();
            // Remove the namespace from the item name
            string pointPattern = @"[^.]*$";
            Match match = Regex.Match(itemName, pointPattern);
            if (match.Success)
            {
                // Split by capital letters. Ej: "MyBrain" -> "My", "Brain"
                string capitalPattern = @"(?=\p{Lu})";
                string[] result = Regex.Split(match.Value, capitalPattern);
                // Join the words in the array with a space
                item.text = string.Join(" ", result);
            }
            // TODO: Style the item using a uss class
            item.style.color = Color.white;
        }
    }
}
