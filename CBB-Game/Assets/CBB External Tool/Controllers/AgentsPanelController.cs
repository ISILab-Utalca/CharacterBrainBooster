using CBB.Api;
using Newtonsoft.Json;
using UnityEngine;
namespace CBB.ExternalTool
{

    public class AgentsPanelController : MonoBehaviour, IMessageHandler
    {
        [SerializeField]
        private MonitoringWindow monitoringWindow;

        private AgentsPanel agentsPanel;
        private HistoryPanel historyPanel;
        
        readonly JsonSerializerSettings settings = new()
        {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Error
        };

        private void Awake()
        {
            monitoringWindow.OnSetupComplete += Initialize;
        }
        private void OnEnable()
        {
            ExternalMonitor.OnMessageReceived += HandleMessage;
        }

        private void OnDisable()
        {
            ExternalMonitor.OnMessageReceived -= HandleMessage;
        }
        private void OnDestroy()
        {
            monitoringWindow.OnSetupComplete -= Initialize;
        }
        private void Initialize()
        {
            agentsPanel = monitoringWindow.AgentsPanel;
            historyPanel = monitoringWindow.HistoryPanel;

        }
        public void HandleMessage(string message)
        {
            try
            {
                var agentWrapper = JsonConvert.DeserializeObject<AgentWrapper>(message, settings);
                // Pass to the model side of the app
                GameData.HandleAgentWrapper(agentWrapper);
                // Update the UI
                agentsPanel.Refresh();
                return;
            }
            catch (System.Exception ex)
            {
                Debug.Log("<color=red>[AGENTS PANEL CONTROLLER] Message is not an AgentWrapper: </color>" + ex);
            }
        }
        
    }

}