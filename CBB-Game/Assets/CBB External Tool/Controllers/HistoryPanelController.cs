using CBB.Lib;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UIElements;
using System.Text.RegularExpressions;
using System.Linq;
using System.ComponentModel;

namespace CBB.ExternalTool
{
    public class HistoryPanelController : MonoBehaviour, IMessageHandler
    {

        public bool logDebug = false;
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

        /*
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
        */

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
            list.itemsSource = packages;
            list.selectionChanged += ShowSelectedDetail;

            var ve = list.Q<VisualElement>("unity-content-container");
            ve.style.flexDirection = FlexDirection.ColumnReverse;
            // Show dropdown
            this.showField = historyPanel.Q<EnumField>();
            showField.value = HistoryPanel.ShowType.Decisions;
            showField.RegisterValueChangedCallback(OnChangeShowType);

            detailPanelController = GetComponent<DetailPanelController>();
            ExternalMonitor.OnMessageReceived += HandleMessage;
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
                if(logDebug) Debug.Log("Selection changed on History Panel to Decision Package\n" +
                    "(Time stamp): " + decision.timestamp + "\nAgent ID: " + decision.agentID);
                detailPanelController.DisplayDecisionDetails(decision);
            }
            else if (selected is SensorPackage sensor)
            {
                if(logDebug) Debug.Log("Selection changed on History Panel to Sensor Package (Time stamp): " + sensor.timestamp);
                detailPanelController.DisplaySensorDetails(sensor);
            }
        }

        public void HandleMessage(string message)
        {
            AgentPackage pack = null;
            try
            {
                pack = JsonConvert.DeserializeObject<DecisionPackage>(message, settings);
                if (pack is DecisionPackage)
                {
                    if (logDebug) Debug.Log("Decision Package received");
                    GameData.HandleDecisionPackage(pack);
                }
            }
            catch (Exception) { }

            try
            {
                pack = JsonConvert.DeserializeObject<SensorPackage>(message, settings);
                if (pack is SensorPackage)
                {
                    if (logDebug) Debug.Log("Sensor Package received");
                    GameData.HandleDecisionPackage(pack);
                }
            }
            catch (Exception) { }

            // Update the view only if necessary
            if (pack?.agentID == currentlySelectedAgentID)
            {
                UpdateHistoryPanelView();
            }
        }
        private void UpdateHistoryPanelView()
        {
            list.Rebuild();
        }
        public void UpdateHistoryPanelView(int agentID)
        {
            //list.ClearClassList();

            currentlySelectedAgentID = agentID;
            packages = GameData.GetHistory(currentlySelectedAgentID);
            list.itemsSource = packages;
            list.Rebuild();
            //list.RefreshItems();
        }

        private VisualElement MakeItem()
        {
            var content = new VisualElement();
            content.style.flexGrow = 0;
            content.focusable = true;
            /*
            var a = new ActionInfo();
            a.name = "Action";
            content.Add(a);
            var s = new SensorInfo();
            s.name = "Sensor";
            content.Add(s);
            */

            return content;
        }

        private void BindItem(VisualElement element, int index)
        {
            switch (showType)
            {
                case HistoryPanel.ShowType.Decisions:
                    if (packages[index] is DecisionPackage desition)
                    {
                        var view = new ActionInfo();
                        element.Add(view);
                        BindActionItem(desition, index, view);
                    }
                    break;
                case HistoryPanel.ShowType.SensorEvents:
                    if (packages[index] is SensorPackage sensor)
                    {
                        var view = new SensorInfo();
                        element.Add(view);
                        BindSensorInfo(sensor, view);
                    }
                    break;
                case HistoryPanel.ShowType.Both:
                    if (packages[index] is DecisionPackage desition2)
                    {
                        var view = new ActionInfo();
                        element.Add(view);
                        BindActionItem(desition2, index, view);
                    }
                    else if (packages[index] is SensorPackage sensor2)
                    {
                        var view = new SensorInfo();
                        element.Add(view);
                        BindSensorInfo(sensor2, view);
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
            list.ClearClassList();

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
            else
            {
                historyPanelTitle.text = "ALL HISTORY";
                detailsPanelTitle.text = "DETAILS";
            }

            UpdateHistoryPanelView(currentlySelectedAgentID);
        }

    }
}

