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
            disconnectButton.clicked += ExternalMonitor.RemoveClient;

            // AgentsPanel
            this.agentsPanel = root.Q<AgentsPanel>();
            agentsPanel.SelectionChange += OnSelectAgent;

            // SimpleBrainView
            this.simpleBrainView = root.Q<SimpleBrainView>();

            // HistoryPanel
            this.historyPanel = root.Q<HistoryPanel>();

            // <Logic>
            // Observe when a new agent is added
            GameData.OnAddAgent += AddAgentToPanel;
            // return to main view after ther server is disconnected
            ExternalMonitor.OnDisconnectedFromServer += ReturnToMainView;
        }

        private void AddAgentToPanel(AgentData wrapper)
        {
            agentsPanel.AddAgent(wrapper);
            Debug.Log("[MONITORING WINDOW] Agent added");
        }

        private void OnSelectAgent(IEnumerable<object> objs)
        {
            var agent = objs.First() as AgentData;

            try
            {
                var history = GameData.GetHistory(agent.ID);
                historyPanel.SetInfo(history);
                historyPanel.Actualize();
            }
            catch
            {
                Debug.Log("Agent " + agent + " has no previous history.");
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