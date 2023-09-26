using CBB.Lib;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using UnityEngine;
using static CBB.ExternalTool.HistoryPanel;
using UnityEngine.UIElements;

namespace CBB.ExternalTool
{
    public class HistoryPanelController : MonoBehaviour, IMessageHandler
    {
        // TODO:
        // Deserialize a Decision Package
        // Update the view after receiving a Decision package
        // Handle OnChange event when selecting a history
        public enum ShowType
        {
            Decisions,
            SensorEvents,
        }
        readonly JsonSerializerSettings settings = new()
        {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Error
        };

        private ObservableCollection<DecisionPackage> decisions;
        private HistoryPanel historyPanel;
        private ListView list;
        private EnumField showField;
        private ShowType showType;
        private void Awake()
        {
            var monitoringWindow = GetComponent<MonitoringWindow>();
            var uiDocRoot = GetComponent<UIDocument>().rootVisualElement;

            this.historyPanel = uiDocRoot.Q<HistoryPanel>();
            this.list = historyPanel.Q<ListView>();
            list.bindItem += BindItem;
            list.makeItem += MakeItem;
            list.itemsSource = decisions;
            list.itemsChosen += OnItemChosen;
            list.selectionChanged += OnSelectionChange;

            // Show dropdown
            this.showField = historyPanel.Q<EnumField>();
            showField.RegisterCallback<ChangeEvent<Enum>>((evt) => OnChangeShowType(evt));
            // cuando este cambie actualiza la lista. y se deseleciona lo selecionado

            ExternalMonitor.OnMessageReceived += HandleMessage;
            // Tabs
            //this.tabs = this.Q<Tabs>();
            //tabs.OnSelectOption += OnSelectionOption;

            GameData.OnAddDecision += AddDecisionInHistory;
        }
        public void HandleMessage(string message)
        {
            try
            {
                var decisionPack = JsonConvert.DeserializeObject<DecisionPackage>(message, settings);
                GameData.HandleDecisionPackage(decisionPack);
                return;
            }
            catch (Exception ex)
            {
                Debug.Log("<color=red>[HISTORY PANEL] Error: </color>" + ex);
            }
        }

        private void AddDecisionInHistory(DecisionPackage package)
        {

        }

        private void OnSelectionOption()
        {

        }
        public void UpdateHistory(int agentID)
        {
            decisions = GameData.GetHistory(agentID);
            list.RefreshItems();
        }

        private VisualElement MakeItem() // hacer que esto sea un solo viewElement (!!!)
        {
            //var content = new VisualElement();
            //var nameLabel = new Label();
            //nameLabel.name = "name";
            //if (showType == ShowType.Both || showType == ShowType.Decisions) // Decision
            //{
            //    /*
            //    var idLabel = new Label();
            //    idLabel.name = "id";
            //    content.Add(idLabel);
            //    content.Add(nameLabel);
            //    */
            //}
            //if (showType == ShowType.Both || showType == ShowType.SensorEvents) // Sensor events
            //{
            //    // Impleentar (!!!)
            //}

            //return content;
            return new Label();
        }

        private void BindItem(VisualElement element, int index)
        {
            if (showType == ShowType.Decisions)
            {
                if (element is ActionInfo actionInfo)
                {
                    actionInfo.ActionName.text = decisions[index].bestOption.actionName;
                    actionInfo.ActionScore.text = decisions[index].bestOption.actionScore.ToString();
                    actionInfo.TargetName.text = decisions[index].bestOption.targetName;
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        /// <summary>
        /// Set the internal list of decisions and refresh the view
        /// </summary>
        /// <param name="history">The new history of decisions to show on the view</param>
        public void SetInfo(ObservableCollection<DecisionPackage> history)
        {
            decisions = history;
            list.RefreshItems();
        }

        private void OnSelectionChange(IEnumerable<object> objs)
        {
            Debug.Log("OnSelectionChange");
        }

        private void OnItemChosen(IEnumerable<object> objs)
        {
            Debug.Log("OnItemChosen");
        }

        private void OnChangeShowType(ChangeEvent<Enum> evt)
        {
            var value = evt.newValue;
            Debug.Log(value.ToString());
        }
        public void LoadAndDisplayAgentHistory(int agentID)
        {
            try
            {
                var history = GameData.GetHistory(agentID);
                SetInfo(history);
            }
            catch
            {
                Debug.Log("[HISTORY PANEL] Agent " + agentID + " has no previous history.");
                return;
            }
        }
    }
}

