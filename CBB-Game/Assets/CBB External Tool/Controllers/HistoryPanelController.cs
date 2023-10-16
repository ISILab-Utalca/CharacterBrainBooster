using CBB.Lib;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

namespace CBB.ExternalTool
{
    public class HistoryPanelController : MonoBehaviour, IMessageHandler
    {
        readonly JsonSerializerSettings settings = new()
        {
            NullValueHandling = NullValueHandling.Include,
            MissingMemberHandling = MissingMemberHandling.Error,
            TypeNameHandling = TypeNameHandling.Auto,
        };
        private DecisionDetailPanelController decisionDetailPanelController;
        private ObservableCollection<DecisionPackage> decisions;
        private HistoryPanel historyPanel;
        private ListView list;
        private EnumField showField;
        private HistoryPanel.ShowType showType;
        private int currentlySelectedAgentID = -1;

        private void Awake()
        {
            var monitoringWindow = GetComponent<MonitoringWindow>();
            var uiDocRoot = GetComponent<UIDocument>().rootVisualElement;

            this.historyPanel = uiDocRoot.Q<HistoryPanel>();
            this.list = historyPanel.Q<ListView>();
            list.bindItem += BindItem;
            list.makeItem += MakeItem;
            list.itemsSource = decisions;
            list.selectionChanged += ShowSelectedDecision;

            // Show dropdown
            this.showField = historyPanel.Q<EnumField>();
            showField.value = HistoryPanel.ShowType.Decisions;
            showField.RegisterValueChangedCallback(OnChangeShowType);

            decisionDetailPanelController = GetComponent<DecisionDetailPanelController>();
            ExternalMonitor.OnMessageReceived += HandleMessage;
        }

        private void ShowSelectedDecision(IEnumerable<object> enumerable)
        {
            var selectedDecision = decisions[list.selectedIndex];
            decisionDetailPanelController.DisplayDecisionDetails(selectedDecision);
        }

        public void HandleMessage(string message)
        {
            try
            {
                var decisionPack = JsonConvert.DeserializeObject<DecisionPackage>(message, settings);
                //Debug.Log("<color=green>[HISTORY PANEL] Message is Decision Pack: </color>" + message);
                GameData.HandleDecisionPackage(decisionPack);

                // Update the view only if necessary
                if (decisionPack.agentID != currentlySelectedAgentID) return;
                list.RefreshItems();
            }
            catch (Exception)
            {
                //Debug.Log("<color=red>[HISTORY PANEL] Message is not Decision Pack: </color>" + message);
            }
        }
        public void UpdateHistoryPanelDecisionsView(int agentID)
        {
            currentlySelectedAgentID = agentID;
            decisions = GameData.GetHistory(currentlySelectedAgentID);
            list.itemsSource = decisions;
            list.RefreshItems();
        }

        private VisualElement MakeItem()
        {
            var content = new VisualElement();

            switch (showType)
            {
                case HistoryPanel.ShowType.Decisions:
                    return new ActionInfo();
                case HistoryPanel.ShowType.SensorEvents:
                    // TODO: return the corresponding type
                    break;
                default:
                    break;
            }

            return content;
        }

        private void BindItem(VisualElement element, int index)
        {
            if (showType == HistoryPanel.ShowType.Decisions)
            {
                if (element is ActionInfo actionInfo)
                {
                    
                    actionInfo.ActionName.text = decisions[decisions.Count - 1 - index].bestOption.actionName;
                    actionInfo.ActionScore.text = decisions[decisions.Count - 1 - index].bestOption.actionScore.ToString();
                    actionInfo.TargetName.text = decisions[decisions.Count - 1 - index].bestOption.targetName;
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
        
        private void OnChangeShowType(ChangeEvent<Enum> evt)
        {
            showType = (HistoryPanel.ShowType)evt.newValue;
        }
        
    }
}

