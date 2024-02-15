using CBB.ExternalTool;
using System;
using UnityEngine;
using UnityEngine.UIElements;

public class ToolsetController : MonoBehaviour
{
    private AgentsPanel m_agentsPanel;
    private HistoryPanel m_historyPanel;
    private BindingsPanel m_bindingsPanel;
    private Button m_agentBrainButton;
    private Button m_historyButton;
    private void Start()
    {
        GetLocalReferences();
        SetAgentBrainClickCallback();
        SetHistoryButtonCallback();
    }

    private void SetHistoryButtonCallback()
    {
        m_historyButton.clicked += HistoryCallback;
    }

    private void HistoryCallback()
    {
        m_historyPanel.style.display = DisplayStyle.Flex;
        m_agentsPanel.style.display = DisplayStyle.Flex;

        // Hide the other panel 
    }

    private void SetAgentBrainClickCallback()
    {
        m_agentBrainButton.clicked += AgentBrainCallback;
    }

    private void AgentBrainCallback()
    {
        m_historyPanel.style.display = DisplayStyle.None;
        m_agentsPanel.style.display = DisplayStyle.None;

        // Display the new panel
    }

    private void GetLocalReferences()
    {
        UIDocument document = GetComponent<UIDocument>();
        VisualElement root = document.rootVisualElement;

        m_agentsPanel = root.Q<AgentsPanel>();
        m_historyPanel = root.Q<HistoryPanel>();
        m_bindingsPanel = root.Q<BindingsPanel>();
        var toolset = root.Q<VisualElement>("toolset");

        m_agentBrainButton = toolset.Q<Button>("bindings-button");
        m_historyButton = toolset.Q<Button>("history-button");
    }
}
