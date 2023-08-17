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

        // View
        private VisualElement infoPanel;
        private VisualElement waitingPanel;
        private DropdownField modeDropdown;
        private Button disconnectButton;
        private AgentsPanel agentsPanel;
        private SimpleBrainView simpleBrainView;
        private HistoryPanel historyPanel;

        // Logic
        [SerializeField]
        private GameObject editorWindow;
        [SerializeField]
        private GameObject mainWindow;

        // Async
        private List<Action> asyncCallBacks = new List<Action>();

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
            disconnectButton.clicked += OnDisconnect;

            // AgentsPanel
            this.agentsPanel = root.Q<AgentsPanel>();
            
            agentsPanel.SelectionChange += OnSelectAgent;

            // SimpleBrainView
            this.simpleBrainView = root.Q<SimpleBrainView>();

            // HistoryPanel
            this.historyPanel = root.Q<HistoryPanel>();

            // Init
            Server.OnClientConnect += (client) => asyncCallBacks.Add(SetWaitingMode);
            Server.OnClientDisconnect += (client) => asyncCallBacks.Add(SetWaitingMode);
            SetWaitingMode();

            GameDataManager.OnClientConnected += InitGameData;
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

        private void Update()
        {
            // Async
            foreach (var callback in asyncCallBacks)
            {
                callback?.Invoke();
            }    
        }

        private void SetWaitingMode()
        {
            var value = (Server.ClientAmount() <= 0);
            infoPanel.SetDisplay(!value);
            waitingPanel.SetDisplay(value);
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

        private void OnDisconnect()
        {
            Server.Stop();
            this.gameObject.SetActive(false);
            this.mainWindow.SetActive(true);
        }
    }
}