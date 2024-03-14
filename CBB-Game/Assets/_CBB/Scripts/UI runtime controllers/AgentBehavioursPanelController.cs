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
		private BrainMapsPanel m_TypeBehavioursPanel;
        private void Awake()
        {
            var uiDoc = GetComponent<UIDocument>();
            m_TypeBehavioursPanel = uiDoc.rootVisualElement.Q<BrainMapsPanel>();
            var dropdown = m_TypeBehavioursPanel.Q<DropdownField>();
            dropdown.RegisterValueChangedCallback(OnValueChanged);
            dropdown.value = "Select an agent type";
            TypeBehavioursHandler_ExternalTool.TypeBehavioursReceived += OnTypeBehavioursReceived;
        }

        private void OnTypeBehavioursReceived()
        {
            Debug.Log("Type Behaviours Received");
            var dropdown = m_TypeBehavioursPanel.Q<DropdownField>();
            var choices = GameData.TypeBehaviours.Select(x => x.agentType);
            dropdown.choices = choices.ToList();
            dropdown.value = choices.First();
        }
        private void OnValueChanged(ChangeEvent<string> evt)
        {
            DisplayTypeBehavioursDetails(evt.newValue);
        }
        private void DisplayTypeBehavioursDetails(string agentType)
        {
            var typeBehaviours = GameData.TypeBehaviours.Where(x => x.agentType == agentType).FirstOrDefault();
            if (typeBehaviours == null)
            {
                Debug.LogError("No brain maps found for the selected agent type");
                return;
            }
            m_TypeBehavioursPanel.ClearBrainMaps();
            foreach (var item in typeBehaviours.subgroups)
            {
                var sgbd = new SubgroupBehaviourDetails(item);
                m_TypeBehavioursPanel.AddBrainMap(sgbd);
                // I have to add the new SubgroupBehaviourDetails element to the panel
            }
        }
    } 
}
