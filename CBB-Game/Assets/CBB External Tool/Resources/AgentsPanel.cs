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
        internal ListView list;
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
            list.itemsSource = GameData.AllAgents;
            list.bindItem += BindItem;
            list.makeItem += MakeItem;
            // Single click triggers "selectionChanged" with the selected items. (f.k.a. "onSelectionChange")
            // Use "selectedIndicesChanged" to get the indices of the selected items instead. (f.k.a. "onSelectedIndicesChange")
            list.selectionChanged += objects => Debug.Log($"Selected: {string.Join(", ", objects)}");

            // Double-click triggers "itemsChosen" with the selected items. (f.k.a. "onItemsChosen")
            list.itemsChosen += objects => Debug.Log($"Double-clicked: {string.Join(", ", objects)}");
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
                agentInfo.AgentName.text = GameData.AllAgents[index].agentName;
                agentInfo.AgentID.text = GameData.AllAgents[index].ID.ToString();
            }
            else
            {
                Debug.Log("[AGENT PANEL] Error on binding element");
            }
        }
        internal void Refresh()
        {
            list.Rebuild();
            Debug.Log("[AGENT PANEL] Agents list updated");
        }
        /// <summary>
        /// Notifies the currently selected agent's ID 
        /// </summary>
        /// <param name="agents"></param>
        private void NewAgentSelected(IEnumerable<object> agents)
        {
            // Positionate the iterator on the selected item
            //    if (agents.First() is VisualElement agentsItem)
            //    {
            //        var idLabel = agentsItem.Q<Label>("id");
            //        if (idLabel != null)
            //        {
            //            OnAgentChosen?.Invoke(int.Parse(idLabel.text));
            //        }
            //    }
            Debug.Log("[AGENT PANEL] On Agent Chosen invoked");
        }
    }
}