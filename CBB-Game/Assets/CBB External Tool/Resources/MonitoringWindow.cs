using CBB.Api;
using CBB.Lib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace CBB.ExternalTool
{
    public class MonitoringWindow : MonoBehaviour
    {
        // Data
        private ExternalMonitor externalMonitor;
        // View
        private VisualElement infoPanel;
        private DropdownField modeDropdown;
        private Button disconnectButton;
        private AgentsPanel agentsPanel;
        private HistoryPanel historyPanel;
        private SimpleBrainView simpleBrainView;

        // Logic
        [SerializeField]
        private GameObject editorWindow;
        [SerializeField]
        private GameObject mainWindow;

        public ExternalMonitor ExternalMonitor { get => externalMonitor; set => externalMonitor = value; }

        private void Awake()
        {

            var root = GetComponent<UIDocument>().rootVisualElement;

            // InfoPanel
            this.infoPanel = root.Q<VisualElement>("InfoPanel");

            // ModeDropdown
            this.modeDropdown = root.Q<DropdownField>("ModeDropdown");
            modeDropdown.RegisterCallback<ChangeEvent<string>>(OnModeChange);

            // DisconnectButton
            this.disconnectButton = root.Q<Button>("DisconnectButton");
            disconnectButton.clicked += ExternalMonitor.RemoveClient;

            // AgentsPanel
            //this.agentsPanel = root.Q<AgentsPanel>();

            // SimpleBrainView
            //this.simpleBrainView = root.Q<SimpleBrainView>();

            // HistoryPanel
            //this.historyPanel = root.Q<HistoryPanel>();
            // <Logic>
            ExternalMonitor.OnDisconnectedFromServer += ReturnToMainView;
            // Show the selected agent history
            //agentsPanel.OnAgentChosen += historyPanel.LoadAndDisplayAgentHistory;
        }

        private void OnModeChange(ChangeEvent<string> evt)
        {
            Debug.Log("OnModeChange");
        }

        private void ReturnToMainView()
        {

            this.gameObject.SetActive(false);
            this.mainWindow.SetActive(true);
        }
    }
}