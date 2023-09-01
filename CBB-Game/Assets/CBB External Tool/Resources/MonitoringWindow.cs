using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using CBB.Comunication;
using CBB.Lib;
using CBB.Api;

namespace CBB.ExternalTool
{
    public class MonitoringWindow : MonoBehaviour
    {
        // Data
        private GameData gameData;
        private ExternalMonitor externalMonitor;
        // View
        private VisualElement infoPanel;
        private VisualElement waitingPanel;
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

        // Async
        private List<Action> asyncCallBacks = new List<Action>();

        public ExternalMonitor ExternalMonitor { get => externalMonitor; set => externalMonitor = value; }

        private void Awake()
        {
            
            var root = GetComponent<UIDocument>().rootVisualElement;

            // InfoPanel
            this.infoPanel = root.Q<VisualElement>("InfoPanel");

            // WaitingPanel
            this.waitingPanel = root.Q<VisualElement>("WaitingPanel");

            // ModeDropdown
            this.modeDropdown = root.Q<DropdownField>("ModeDropdown");
            modeDropdown.RegisterCallback<ChangeEvent<string>>(OnModeChange);

            // DisconnectButton
            this.disconnectButton = root.Q<Button>("DisconnectButton");
            disconnectButton.clicked += ReturnToMainView;
            disconnectButton.clicked += ExternalMonitor.RemoveClient;

            // AgentsPanel
            this.agentsPanel = root.Q<AgentsPanel>();
            
            agentsPanel.SelectionChange += OnSelectAgent;

            // SimpleBrainView
            this.simpleBrainView = root.Q<SimpleBrainView>();

            // HistoryPanel
            this.historyPanel = root.Q<HistoryPanel>();

            GameDataManager.OnClientConnected += InitGameData;
            // Handle server desconection
            ExternalMonitor.OnDisconnectedFromServer += ReturnToMainView;
        }

        private void InitGameData(GameData gameData)
        {
            this.gameData = gameData;
            gameData.OnAddAgent += AddAgentToPanel;
        }

        private void AddAgentToPanel(GameData data, AgentWrapper wrapper)
        {
            agentsPanel.AddAgent(data, wrapper);
        }

        private void OnSelectAgent(IEnumerable<object> objs)
        {
            var agent = objs.First() as AgentWrapper;

            try
            {
                var history = gameData.GetHistory(agent);
                historyPanel.SetInfo(history);
                historyPanel.Actualize();
            }
            catch
            {
                Debug.Log("Agent "+ agent +" has no previous history.");
                return;
            }
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