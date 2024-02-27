using CBB.Api;
using CBB.Lib;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace CBB.ExternalTool
{

    public class AgentsPanelController : MonoBehaviour, IMessageHandler
    {
        [SerializeField]
        private bool showLogs;

        private AgentsPanel agentsPanel;
        internal ListView list;

        // For some reason I do not understand yet, new GameObjects are created when the game is played
        // if the deserialization settings are different from the ones declared here. (27/Feb/2024)
        readonly JsonSerializerSettings settings = new()
        {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Error
        };

        public System.Action<int> OnNewAgentSelected { get; set; }
        private void Awake()
        {

            var uiDocRoot = GetComponent<UIDocument>().rootVisualElement;

            this.agentsPanel = uiDocRoot.Q<AgentsPanel>();
            this.list = agentsPanel.Q<ListView>();
            list.itemsSource = GameData.Agent_ID_Name;
            list.bindItem += BindItem;
            list.makeItem += MakeItem;
            list.selectionChanged += NewAgentSelected;

            GameData.OnAddAgent += Refresh;
        }
        private void OnEnable()
        {
            ExternalMonitor.OnMessageReceived += HandleMessage;
        }

        private void OnDisable()
        {
            ExternalMonitor.OnMessageReceived -= HandleMessage;
        }

        private VisualElement MakeItem()
        {
            return new AgentInfo();
        }
        private void BindItem(VisualElement element, int index)
        {
            if (element is AgentInfo agentInfo)
            {
                agentInfo.AgentName.text = GameData.Agent_ID_Name[index].Item2;
                agentInfo.AgentID.text = $"ID: {GameData.Agent_ID_Name[index].Item1}";
            }
        }
        internal void Refresh(AgentData agent)
        {
            list.Rebuild();
            if (showLogs) Debug.Log("[AGENT PANEL] Agents list updated");
        }
        private void NewAgentSelected(IEnumerable<object> agents)
        {
            var agentID = (((int, string))agents.First()).Item1;
            OnNewAgentSelected?.Invoke(agentID);
        }
        public void HandleMessage(string message)
        {
            try
            {
                var agentWrapper = JsonConvert.DeserializeObject<AgentWrapper>(message, settings);
                // Pass to the model side of the app
                GameData.HandleAgentWrapper(agentWrapper);

                if (showLogs)
                {
                    Debug.Log($"AgentWrapper received on {gameObject.name}:{this.name}");
                }
            }
            catch (System.Exception) { }
        }
    }
}