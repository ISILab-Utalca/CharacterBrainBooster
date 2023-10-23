using CBB.Lib;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UIElements;
using System.Text.RegularExpressions;
using UnityEditor;
using System.Linq;

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
        private DetailPanelController detailPanelController;
        private ObservableCollection<AgentPackage> packages;
        private HistoryPanel historyPanel;
        private ListView list;
        private EnumField showField;
        private HistoryPanel.ShowType showType;
        private int currentlySelectedAgentID = -1;
        private Label historyPanelTitle;
        private Label detailsPanelTitle;


        private ObservableCollection<AgentPackage> Packages
        {
            get
            {
                if(showType == HistoryPanel.ShowType.Decisions)
                {
                    return new ObservableCollection<AgentPackage>(packages.Where(x => x is DecisionPackage));
                }
                else if(showType == HistoryPanel.ShowType.SensorEvents)
                {
                    return new ObservableCollection<AgentPackage>(packages.Where(x => x is SensorPackage));
                }
                else
                {
                    return packages;
                }
            }
        }

        private void Awake()
        {
            var monitoringWindow = GetComponent<MonitoringWindow>();
            var uiDocRoot = GetComponent<UIDocument>().rootVisualElement;

            this.historyPanel = uiDocRoot.Q<HistoryPanel>();
            this.historyPanelTitle = historyPanel.Q<Label>("title");
            this.detailsPanelTitle = uiDocRoot.Q<Label>("details-panel-title");
            this.list = historyPanel.Q<ListView>();
            list.bindItem += BindItem;
            list.makeItem += MakeItem;
            list.itemsSource = Packages;
            list.selectionChanged += ShowSelectedDetail;

            // Show dropdown
            this.showField = historyPanel.Q<EnumField>();
            showField.value = HistoryPanel.ShowType.Decisions;
            showField.RegisterValueChangedCallback(OnChangeShowType);

            detailPanelController = GetComponent<DetailPanelController>();
            ExternalMonitor.OnMessageReceived += HandleMessage;
        }

        private void ShowSelectedDetail(IEnumerable<object> enumerable)
        {
            var selected = Packages[Packages.Count - 1 - list.selectedIndex];

            if(selected is DecisionPackage decision)
            {
                detailPanelController.DisplayDecisionDetails(selected as DecisionPackage);
            }
            else if(selected is SensorPackage)
            {
                // do something cool
            }
        }

        public void HandleMessage(string message)
        {
            try
            {
                var decisionPack = JsonConvert.DeserializeObject<DecisionPackage>(message, settings);
                //Debug.Log("<color=green>[HISTORY PANEL] Message is Decision Pack: </color>" + message);
                GameData.HandleDecisionPackage(decisionPack);

                // Update the view only if necessary
                if (decisionPack.agentID != currentlySelectedAgentID) 
                    return;

                list.RefreshItems();
            }
            catch (Exception)
            {
                //Debug.Log("<color=red>[HISTORY PANEL] Message is not Decision Pack: </color>" + message);
            }
        }
        public void UpdateHistoryPanelView(int agentID)
        {
            currentlySelectedAgentID = agentID;
            packages = GameData.GetHistory(currentlySelectedAgentID);
            list.itemsSource = Packages;
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
                    return new SensorInfo();
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

                    var reverseIndex = Packages.Count - 1 - index;
                    var decision = Packages[reverseIndex] as DecisionPackage;
                    var actionTypeName = decision.bestOption.actionName;
                    string[] words = Regex.Split(actionTypeName, @"(?<!^)(?=[A-Z][a-z])");

                    actionInfo.ActionName.text = string.Join(" ", words); ;

                    actionInfo.ActionScore.text = decision.bestOption.actionScore.ToString();
                    actionInfo.TargetName.text = decision.bestOption.targetName;

                    var t = decision.timestamp;
                    var tt = DateTime.Parse(t);
                    actionInfo.TimeStamp.text = tt.ToString("HH:mm:ss");
                    actionInfo.ActionID.text = $"ID: {reverseIndex}";
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            else if(showType == HistoryPanel.ShowType.SensorEvents)
            {
                if(element is SensorInfo sensorInfo)
                {
                    var sensor = Packages[Packages.Count - 1 - index] as SensorPackage;

                    sensorInfo.TimeStamp.text = sensor.timestamp;
                    sensorInfo.SensorName.text = sensor.test;
                    sensorInfo.ExtraInfo.text = sensor.test;
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
            // Update titles
            if (showType == HistoryPanel.ShowType.Decisions)
            {
                historyPanelTitle.text = "DECISIONS HISTORY";
                detailsPanelTitle.text = "DECISION DETAILS";
            }
            else if (showType == HistoryPanel.ShowType.SensorEvents)
            {
                historyPanelTitle.text = "SENSOR EVENTS HISTORY";
                detailsPanelTitle.text = "SENSOR EVENT DETAILS";
            }
        }

    }
}

