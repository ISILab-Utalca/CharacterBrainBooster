using CBB.Comunication;
using CBB.UI;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace CBB.ExternalTool
{
    public class TypeBehavioursPanelController : MonoBehaviour
	{
		private TypeBehavioursPanel m_TypeBehavioursPanel;
        private void Awake()
        {
            var uiDoc = GetComponent<UIDocument>();
            m_TypeBehavioursPanel = uiDoc.rootVisualElement.Q<TypeBehavioursPanel>();
            var dropdown = m_TypeBehavioursPanel.Q<DropdownField>();
            dropdown.RegisterValueChangedCallback(OnValueChanged);
            dropdown.value = "Select an agent type";
            TypeBehavioursHandler_ExternalTool.TypeBehavioursReceived += OnTypeBehavioursReceived;
            HandleFloatingPanel();
        }

        private void HandleFloatingPanel()
        {
            // Close floating panel if the mouse is clicked outside of it
            m_TypeBehavioursPanel.panel.visualTree.RegisterCallback<MouseDownEvent>(evt =>
            {
                var evtFP = evt.target as MoveAgentFloatingPanel;
                var evtFPLI = evt.target as SubgroupListItem;
                if (evtFP != null || evtFPLI != null) return;

                CloseFloatingPanels();
            });
        }
        private void CloseFloatingPanels()
        {
            var floatingPanels = m_TypeBehavioursPanel.Q<MoveAgentFloatingPanel>();
            floatingPanels?.Close();
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
            m_TypeBehavioursPanel.ClearContent();
            foreach (var item in typeBehaviours.subgroups)
            {
                SubgroupBehaviourView subgroupDetails = new(item)
                {
                    userData = m_TypeBehavioursPanel.AgentTypes.value
                };
                m_TypeBehavioursPanel.AddContent(subgroupDetails);
            }
        }
    } 
}
