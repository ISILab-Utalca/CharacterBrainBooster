using CBB.Lib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace CBB.ExternalTool
{
    public class HistoryPanel : VisualElement
    {
        #region FACTORY
        public new class UxmlFactory : UxmlFactory<HistoryPanel, VisualElement.UxmlTraits> { }
        #endregion

        public enum ShowType
        {
            Decisions,
            SensorEvents,
        }

        // View
        private ListView list;
        private EnumField showField;
        private Tabs tabs;

        // Info
        private ShowType showType = ShowType.Decisions;
        private List<DecisionPackage> decisions;
        // TODO: List for the sensor events
        public HistoryPanel()
        {
            var visualTree = Resources.Load<VisualTreeAsset>("HistoryPanel");
            visualTree.CloneTree(this);

            // History list
            this.list = this.Q<ListView>();
            list.bindItem += BindItem;
            list.makeItem += MakeItem;
            list.itemsSource = decisions;
            list.itemsChosen += OnItemChosen;
            list.selectionChanged += OnSelectionChange;

            // Show dropdown
            this.showField = this.Q<EnumField>();
            showField.RegisterCallback<ChangeEvent<Enum>>((evt) => OnChangeShowType(evt));
            // cuando este cambie actualiza la lista. y se deseleciona lo selecionado

            // Tabs
            //this.tabs = this.Q<Tabs>();
            //tabs.OnSelectOption += OnSelectionOption;

            GameData.OnAddDecision += AddDecisionInHistory;
        }

        private void AddDecisionInHistory(DecisionPackage package)
        {

        }

        private void OnSelectionOption()
        {

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
            return showType switch
            {
                ShowType.Decisions => new ActionInfo(),
                ShowType.SensorEvents => throw new NotImplementedException(),//TODO
                _ => null,
            };
        }

        private void BindItem(VisualElement element, int index)
        {
            if(showType == ShowType.Decisions)
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
        public void SetInfo(List<DecisionPackage> history)
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