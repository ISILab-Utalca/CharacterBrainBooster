using CBB.ExternalTool;
using System;
using UnityEngine;
using UnityEngine.UIElements;

public class ToolsetController : MonoBehaviour
{
    private SplitView m_splitView;
    private BindingsPanel m_bindingsPanel;
    private Button m_agentBrainButton;
    private Button m_historyButton;
    private void Start()
    {
        GetLocalReferences();
        SetBindingCallback();
        SetHistoryCallback();
    }

    private void SetHistoryCallback()
    {
        m_historyButton.clicked += AgentsHistoryCallback;
    }

    private void AgentsHistoryCallback()
    {
        m_splitView.style.display = DisplayStyle.Flex;
        // Hide the other panel 
        m_bindingsPanel.style.display = DisplayStyle.None;
    }

    private void SetBindingCallback()
    {
        m_agentBrainButton.clicked += BindingCallback;
    }

    private void BindingCallback()
    {
        m_splitView.style.display = DisplayStyle.None;

        // Display the new panel
        m_bindingsPanel.style.display = DisplayStyle.Flex;
    }

    private void GetLocalReferences()
    {
        UIDocument document = GetComponent<UIDocument>();
        VisualElement root = document.rootVisualElement;

        m_splitView = root.Q<SplitView>("agents-history-panel-container");
        m_bindingsPanel = root.Q<BindingsPanel>();
        var toolset = root.Q<VisualElement>("toolset");

        m_agentBrainButton = toolset.Q<Button>("bindings-button");
        m_historyButton = toolset.Q<Button>("history-button");
    }
}
