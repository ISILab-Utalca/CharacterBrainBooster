using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


namespace CBB.ExternalTool
{
    public class EditorWindowController : MonoBehaviour
    {
        // UI
        private TreeView brainTree;

        // Logic
        [SerializeField]
        private BrainDataManager brainDataManager;

        private void Awake()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            brainTree = root.Q<TreeView>("brain-tree");
        }
        /// <summary>
        /// Retrieve all the brains existing in the game
        /// </summary>
        public void LoadBrains()
        {
            var brains = brainDataManager.GetBrains();
            // Display the brains in the tree view
            brainTree.Clear();
            // Expresses planet data as a list of TreeViewItemData objects. Needed for TreeView and MultiColumnTreeView.
            //protected static IList<TreeViewItemData<IPlanetOrGroup>> treeRoots
            //    {
            //        get
            //        {
            //            int id = 0;
            //            var roots = new List<TreeViewItemData<IPlanetOrGroup>>(planetGroups.Count);
            //            foreach (var group in planetGroups)
            //            {
            //                var planetsInGroup = new List<TreeViewItemData<IPlanetOrGroup>>(group.planets.Count);
            //                foreach (var planet in group.planets)
            //                {
            //                    planetsInGroup.Add(new TreeViewItemData<IPlanetOrGroup>(id++, planet));
            //                }

            //                roots.Add(new TreeViewItemData<IPlanetOrGroup>(id++, group, planetsInGroup));
            //            }
            //            return roots;
            //        }
            //    }
        }
        private void OnEnable()
        {
        }
    }
}
