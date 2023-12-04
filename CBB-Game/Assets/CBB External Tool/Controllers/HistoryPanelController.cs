using CBB.Lib;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UIElements;
using System.Text.RegularExpressions;
using System.Linq;

namespace CBB.ExternalTool
{
    public class HistoryPanelController : MonoBehaviour, IMessageHandler
    {

        public bool showLogs = false;
        readonly JsonSerializerSettings settings = new()
        {
            NullValueHandling = NullValueHandling.Include,
            MissingMemberHandling = MissingMemberHandling.Error,
            TypeNameHandling = TypeNameHandling.Auto,
        };
        private DetailPanelController detailPanelController;
        private HistoryPanel historyPanel;
        private ListView list;
        private EnumField showField;
        private HistoryPanel.ShowType showType;
        private int currentlySelectedAgentID = -1;
        private Label historyPanelTitle;
        private Label detailsPanelTitle;

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
            
            list.selectionChanged += ShowSelectedDetail;
            list.selectedIndicesChanged += ShowSelectedDetail;
            // Show dropdown
            this.showField = historyPanel.Q<EnumField>();
            showField.value = HistoryPanel.ShowType.Decisions;
            showField.RegisterValueChangedCallback(OnChangeShowType);

            detailPanelController = GetComponent<DetailPanelController>();
            ExternalMonitor.OnMessageReceived += HandleMessage;
        }

        private void ShowSelectedDetail(IEnumerable<int> enumerable)
        {
            int rIndex = list.itemsSource.Count - enumerable.First() - 1;
        }

        private void ShowSelectedDetail(IEnumerable<object> enumerable)
        {
            if (enumerable.Count() <= 0)
            {
                detailPanelController.ClearDetails();
                return;
            }
            var selected = enumerable.ToArray()[0];

            if (selected is DecisionPackage decision)
            {
                if(showLogs) Debug.Log("Selection changed on History Panel to Decision Package\n" +
                    "(Time stamp): " + decision.timestamp + "\nAgent ID: " + decision.agentID);
                detailPanelController.DisplayDecisionDetails(decision);
            }
            else if (selected is SensorPackage sensor)
            {
                if(showLogs) Debug.Log("Selection changed on History Panel to Sensor Package (Time stamp): " + sensor.timestamp);
                detailPanelController.DisplaySensorDetails(sensor);
            }
        }

        public void HandleMessage(string message)
        {
            AgentPackage pack = null;
            try
            {
                pack = JsonConvert.DeserializeObject<DecisionPackage>(message, settings);
                if (pack is DecisionPackage dp)
                {
                    if (showLogs) Debug.Log("Decision Package received");
                    GameData.HandleDecisionPackage(dp);
                }
            }
            catch (Exception) { }
            try
            {
                pack = JsonConvert.DeserializeObject<SensorPackage>(message, settings);
                if (pack is SensorPackage sp)
                {
                    if (showLogs) Debug.Log("Sensor Package received");
                    GameData.HandleSensorEventPackage(sp);
                }
            }
            catch (Exception) { }

            if (pack?.agentID != currentlySelectedAgentID) return;
            UpdateHistoryPanelView();
        }
        private void UpdateHistoryPanelView()
        {
            switch (showType)
            {
                case HistoryPanel.ShowType.Both:
                    list.itemsSource = GameData.GetAgentFullHistory(currentlySelectedAgentID);
                    break;
                case HistoryPanel.ShowType.Decisions:
                    list.itemsSource = GameData.GetAgentDecisions(currentlySelectedAgentID);
                    break;
                case HistoryPanel.ShowType.SensorEvents:
                    list.itemsSource = GameData.GetAgentSensorEvents(currentlySelectedAgentID);
                    break;
            }
            list.Rebuild();
        }
        
        public void UpdateHistoryPanelView(int agentID)
        {
            currentlySelectedAgentID = agentID;
            UpdateHistoryPanelView();
        }

        private VisualElement MakeItem()
        {
            var content = new VisualElement();
            content.style.flexGrow = 1;
            content.focusable = true;

            return content;
        }

        private void BindItem(VisualElement element, int index)
        {
            int rIndex = list.itemsSource.Count - index - 1;
            switch (showType)
            {
                case HistoryPanel.ShowType.Decisions:
                    var view = new ActionInfo();
                    element.Add(view);
                    BindActionItem(list.itemsSource[rIndex] as DecisionPackage, rIndex, view);
                    break;
                case HistoryPanel.ShowType.SensorEvents:
                    if (list.itemsSource[rIndex] is SensorPackage sensor)
                    {
                        SensorInfo si = new();
                        element.Add(si);
                        BindSensorInfo(sensor, si);
                    }
                    break;
                case HistoryPanel.ShowType.Both:
                    if (list.itemsSource[rIndex] is DecisionPackage desition2)
                    {
                        var ai = new ActionInfo();
                        element.Add(ai);
                        BindActionItem(desition2, index, ai);
                    }
                    else if (list.itemsSource[rIndex] is SensorPackage sensor2)
                    {
                        SensorInfo si = new();
                        element.Add(si);
                        BindSensorInfo(sensor2, si);
                    }
                    break;
            }
        }

        private void BindSensorInfo(SensorPackage sensor, SensorInfo sensorInfo)
        {
            sensorInfo.style.display = DisplayStyle.Flex;

            var t = sensor.timestamp;
            var tt = DateTime.Parse(t);
            sensorInfo.TimeStamp.text = tt.ToString("HH:mm:ss");
            sensorInfo.SensorName.text = sensor.sensorType;
            sensorInfo.ExtraInfo.text = sensor.extraData;
        }
        
        private void BindActionItem(DecisionPackage decision, int index, ActionInfo actionPanel)
        {
            actionPanel.style.display = DisplayStyle.Flex;

            var actionTypeName = decision.bestOption.actionName;
            string[] words = Regex.Split(actionTypeName, @"(?<!^)(?=[A-Z][a-z])");

            actionPanel.ActionName.text = string.Join(" ", words); ;

            actionPanel.ActionScore.text = decision.bestOption.actionScore.ToString();
            actionPanel.TargetName.text = decision.bestOption.targetName;

            var t = decision.timestamp;
            var tt = DateTime.Parse(t);
            actionPanel.TimeStamp.text = tt.ToString("HH:mm:ss");
            actionPanel.ActionID.text = $"ID: {index}";
        }

        private void OnChangeShowType(ChangeEvent<Enum> evt)
        {
            //list.ClearClassList();

            showType = (HistoryPanel.ShowType)evt.newValue;
            // Update titles
            switch (showType)
            {
                case HistoryPanel.ShowType.Decisions:
                    historyPanelTitle.text = "DECISIONS HISTORY";
                    detailsPanelTitle.text = "DECISION DETAILS";
                    break;
                case HistoryPanel.ShowType.SensorEvents:
                    historyPanelTitle.text = "SENSOR EVENTS HISTORY";
                    detailsPanelTitle.text = "SENSOR EVENT DETAILS";
                    break;
                default:
                    historyPanelTitle.text = "ALL HISTORY";
                    detailsPanelTitle.text = "DETAILS";
                    break;
            }

            UpdateHistoryPanelView();
        }

    }
}

