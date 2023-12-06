using UnityEngine;
using UnityEngine.UIElements;

namespace CBB.ExternalTool
{
    public class BrainEditorController : MonoBehaviour
    {
        private ListView brainList;
        private void Awake()
        {
            // Get reference to the brain List
            var root = GetComponent<UIDocument>().rootVisualElement;

            brainList = root.Q<ListView>("brain-list");
        }
        public void LoadBrains()
        {
            // Clear the list
            brainList.Clear();

            // Load all brains
            var brains = DataLoader.GetAllBrains();

            // Add each brain to the list
            foreach (var brain in brains)
            {
                var brainCard = new BrainCard();
                //brainCard.brainName.text = brain.brain_name;
                brainCard.brainID.text = brain.brain_ID;
                brainList.Add(brainCard);
            }
        }
    }
}