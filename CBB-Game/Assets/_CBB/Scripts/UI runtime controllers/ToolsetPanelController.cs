using CBB.UI;
using UnityEngine;
using UnityEngine.UIElements;

public class ToolsetPanelController : MonoBehaviour
{
    private SplitView m_splitView;
    private TypeBehavioursPanel m_brainMapsPanel;
    private Button m_agentBrainButton;
    private Button m_historyButton;
    private VisualElement m_brainBottomLine;
    private VisualElement m_historyBottomLine;
    private void Awake()
    {
        GetLocalReferences();
        SetBindingCallback();
        SetHistoryCallback();
        AgentsHistoryCallback();
    }
    private void GetLocalReferences()
    {
        UIDocument document = GetComponent<UIDocument>();
        VisualElement root = document.rootVisualElement;

        m_splitView = root.Q<SplitView>("agents-history-panel-container");
        m_brainMapsPanel = root.Q<TypeBehavioursPanel>();
        var toolset = root.Q<VisualElement>("toolset");

        m_agentBrainButton = toolset.Q<Button>("bindings-button");
        m_historyButton = toolset.Q<Button>("history-button");

        m_brainBottomLine = toolset.Q<VisualElement>("agent-brain-bindings_bottom-line");
        m_historyBottomLine = toolset.Q<VisualElement>("history_bottom-line");
    }
    private void SetBindingCallback()
    {
        m_agentBrainButton.clicked += BindingCallback;
    }
    private void BindingCallback()
    {
        m_splitView.style.display = DisplayStyle.None;
        m_brainBottomLine.style.display = DisplayStyle.Flex;
        m_historyBottomLine.style.display = DisplayStyle.None;
        // Display the new panel
        m_brainMapsPanel.style.display = DisplayStyle.Flex;
    }
    private void SetHistoryCallback()
    {
        m_historyButton.clicked += AgentsHistoryCallback;
    }
    private void AgentsHistoryCallback()
    {
        m_splitView.style.display = DisplayStyle.Flex;
        m_brainBottomLine.style.display = DisplayStyle.None;
        m_historyBottomLine.style.display = DisplayStyle.Flex;
        // Hide the other panel 
        m_brainMapsPanel.style.display = DisplayStyle.None;
    }
}
