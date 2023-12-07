using UnityEngine;
using UnityEngine.UIElements;


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
            (element as Label).text = brainTree.GetItemDataForIndex<IDataItem>(index).GetItemName();
        }
    }
}
