using CBB.Comunication;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace CBB.ExternalTool
{
    public class AgentBehavioursPanelController : MonoBehaviour
	{
		private BindingsPanel m_bindingsPanel;
        private void Awake()
        {
            var uiDoc = GetComponent<UIDocument>();
            m_bindingsPanel = uiDoc.rootVisualElement.Q<BindingsPanel>();
            var dropdown = m_bindingsPanel.Q<DropdownField>();
            dropdown.RegisterValueChangedCallback(OnValueChanged);
            dropdown.value = "Select an agent type";
            BrainMapsHandler_ExternalTool.BrainMapsReceived += OnBrainMapsReceived;
        }

        private void OnBrainMapsReceived()
        {
            Debug.Log("Brain Maps Received");
            var dropdown = m_bindingsPanel.Q<DropdownField>();
            var choices = BrainMapsHandler_ExternalTool.BrainMaps.Select(x => x.agentType);
            dropdown.choices = choices.ToList();
        }

        private void OnValueChanged(ChangeEvent<string> evt)
        {
            
        }
    } 
}
