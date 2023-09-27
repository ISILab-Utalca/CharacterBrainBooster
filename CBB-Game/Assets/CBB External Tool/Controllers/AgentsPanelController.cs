using CBB.Api;
using CBB.Lib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace CBB.ExternalTool
{

    public class AgentsPanelController : MonoBehaviour, IMessageHandler
    {
        private MonitoringWindow monitoringWindow;
        private HistoryPanelController historyPanel;
        private AgentsPanel agentsPanel;
        internal ListView list;

        readonly JsonSerializerSettings settings = new()
        {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Error
        };

        private void Awake()
        {
            monitoringWindow = GetComponent<MonitoringWindow>();
            historyPanel = GetComponent<HistoryPanelController>();

            var uiDocRoot = GetComponent<UIDocument>().rootVisualElement;
            
            this.agentsPanel = uiDocRoot.Q<AgentsPanel>();
            this.list = agentsPanel.Q<ListView>();
            list.itemsSource = GameData.Agent_ID_Name;
            list.bindItem += BindItem;
            list.makeItem += MakeItem;
            list.selectionChanged += NewAgentSelected;

            //list.itemsChosen += NewAgentSelected;

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
        
        private VisualElement MakeItem() // hacer que esto sea un solo viewElement (!!!)
        {
            return new AgentInfo();
        }
        private void BindItem(VisualElement element, int index)
        {
            if (element is AgentInfo agentInfo)
            {
                agentInfo.AgentID.text = GameData.Agent_ID_Name[index].Item1.ToString();
                agentInfo.AgentName.text = GameData.Agent_ID_Name[index].Item2;
            }
            
        }
        internal void Refresh(AgentData agent)
        {
            list.RefreshItems();
            //Debug.Log("[AGENT PANEL] Agents list updated");
        }
        private void NewAgentSelected(IEnumerable<object> agents)
        {
            // Go to the History panel and update its list, based on the selected agent ID
            historyPanel.UpdateHistoryPanelDecisionsView( (((int,string))agents.First()).Item1 );
        }
        public void HandleMessage(string message)
        {
            try
            {
                var agentWrapper = JsonConvert.DeserializeObject<AgentWrapper>(message, settings);
                // Pass to the model side of the app
                GameData.HandleAgentWrapper(agentWrapper);
                // Update the UI
                //if (agentWrapper.type == AgentWrapper.AgentStateType.NEW)
                list.RefreshItems();
                return;
            }
            catch (System.Exception)
            {
                //Debug.Log("<color=red>[AGENTS PANEL CONTROLLER] Message is not an AgentWrapper: </color>" + ex);
            }
        }

    }

}