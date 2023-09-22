using CBB.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace CBB.ExternalTool
{
    public class AgentsPanel : VisualElement
    {
        #region FACTORY
        public new class UxmlFactory : UxmlFactory<AgentsPanel, UxmlTraits> { }
        #endregion
        #region FIELDS
        private int bindItemCalls = 0;
        private ListView list;
        private List<(string, int)> targetAgentAndID = new();
        #endregion
        #region EVENTS
        public Action<int> OnAgentChosen { get; set; }
        #endregion
        #region CONSTRUCTOR
        public AgentsPanel()
        {
            var visualTree = Resources.Load<VisualTreeAsset>("AgentsPanel");
            visualTree.CloneTree(this);

            this.list = this.Q<ListView>();
            list.itemsSource = targetAgentAndID;
            list.bindItem += BindItem;
            list.makeItem += MakeItem;
            list.selectionType = SelectionType.Single;
            list.selectionChanged += NewAgentSelected;
            GameData.OnAddAgent += AddAgent;
        }
        #endregion

        private VisualElement MakeItem() // hacer que esto sea un solo viewElement (!!!)
        {
            return new AgentInfo();
        }
        private void BindItem(VisualElement element, int index)
        {
            var agentInfo = element as AgentInfo;
            if (agentInfo != null)
            {
                agentInfo.AgentName.text = targetAgentAndID[index].Item1;
                agentInfo.AgentID.text = targetAgentAndID[index].Item2.ToString();
            }
            else
            {
                Debug.Log("[AGENT PANEL] Error on binding element");
            }
            bindItemCalls++;
            Debug.Log($"[AGENTS PANEL] Called bind item {bindItemCalls}");
        }
        internal void AddAgent(AgentData agent)
        {
            if (targetAgentAndID.Contains((agent.agentName, agent.ID))) return;
            targetAgentAndID.Add((agent.agentName, agent.ID));

            // Call this method to update the view on runtime
            //list.RefreshItems();
            list.Rebuild();
            Debug.Log("[AGENT PANEL] Added agent");
        }
        /// <summary>
        /// Notifies the currently selected agent's ID 
        /// </summary>
        /// <param name="agents"></param>
        private void NewAgentSelected(IEnumerable<object> agents)
        {
            // Positionate the iterator on the selected item
            if (agents.First() is VisualElement agentsItem)
            {
                var idLabel = agentsItem.Q<Label>("id");
                if (idLabel != null)
                {
                    OnAgentChosen?.Invoke(int.Parse(idLabel.text));
                    Debug.Log("[AGENT PANEL] On Agent Chosen invoked");
                }
            }
        }
    }
}