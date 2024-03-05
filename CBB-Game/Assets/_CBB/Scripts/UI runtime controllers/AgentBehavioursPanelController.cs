using CBB.Comunication;
using CBB.UI;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace CBB.ExternalTool
{
    public class AgentBehavioursPanelController : MonoBehaviour
	{
		private BrainMapsPanel m_brainMapsPanel;
        private void Awake()
        {
            var uiDoc = GetComponent<UIDocument>();
            m_brainMapsPanel = uiDoc.rootVisualElement.Q<BrainMapsPanel>();
            var dropdown = m_brainMapsPanel.Q<DropdownField>();
            dropdown.RegisterValueChangedCallback(OnValueChanged);
            dropdown.value = "Select an agent type";
            BrainMapsHandler_ExternalTool.BrainMapsReceived += OnBrainMapsReceived;
        }

        private void OnBrainMapsReceived()
        {
            Debug.Log("Brain Maps Received");
            var dropdown = m_brainMapsPanel.Q<DropdownField>();
            var choices = GameData.BrainMaps.Select(x => x.agentType);
            dropdown.choices = choices.ToList();
            dropdown.value = choices.First();
        }
        private void OnValueChanged(ChangeEvent<string> evt)
        {
            DisplayBrainMapsDetails(evt.newValue);
        }
        private void DisplayBrainMapsDetails(string agentType)
        {
            var bm = GameData.BrainMaps.Where(x => x.agentType == agentType).FirstOrDefault();
            if (bm == null)
            {
                Debug.LogError("No brain maps found for the selected agent type");
                return;
            }
            m_brainMapsPanel.ClearBrainMaps();
            foreach (var item in bm.SubgroupsBrains)
            {
                var sgbd = new SubgroupBehaviourDetails(item);
                m_brainMapsPanel.AddBrainMap(sgbd);
                // I have to add the new SubgroupBehaviourDetails element to the panel
            }
        }
    } 
}
