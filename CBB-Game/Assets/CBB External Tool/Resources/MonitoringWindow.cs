using CBB.Api;
using CBB.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace CBB.ExternalTool
{
    public class MonitoringWindow : MonoBehaviour
    {
        // View
        private DropdownField modeDropdown;
        private Button disconnectButton;
        private SimpleBrainView simpleBrainView;
        #region PROPERTIES
        public AgentsPanel AgentsPanel { get; private set; }
        public HistoryPanel HistoryPanel { get; private set; }

        #endregion
        #region EVENTS
        public System.Action OnSetupComplete { get; set; }
        public System.Action OnDisconnectedFromServer { get; set; }
        #endregion
        private void OnEnable()
        {

            var root = GetComponent<UIDocument>().rootVisualElement;

            // ModeDropdown
            this.modeDropdown = root.Q<DropdownField>("ModeDropdown");
            modeDropdown.RegisterCallback<ChangeEvent<string>>(OnModeChange);

            // DisconnectButton
            this.disconnectButton = root.Q<Button>("DisconnectButton");
            disconnectButton.clicked += CloseMonitoringWindow;

            // AgentsPanel
            this.AgentsPanel = root.Q<AgentsPanel>();

            // SimpleBrainView
            //this.simpleBrainView = root.Q<SimpleBrainView>();

            //HistoryPanel
            this.HistoryPanel = root.Q<HistoryPanel>();
            // Show the selected agent history
            //agentsPanel.OnAgentChosen += historyPanel.LoadAndDisplayAgentHistory;
            OnSetupComplete?.Invoke();
        }

        private void CloseMonitoringWindow()
        {
            OnDisconnectedFromServer?.Invoke();
        }

        private void OnDisable()
        {
            modeDropdown.UnregisterCallback<ChangeEvent<string>>(OnModeChange);
            disconnectButton.clicked -= CloseMonitoringWindow;
        }
        private void OnModeChange(ChangeEvent<string> evt)
        {
            Debug.Log("OnModeChange");
        }

    }
}