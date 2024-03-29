using CBB.Lib;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DetailPanelController : MonoBehaviour
{
    ListView list;
    List<DecisionData> auxDecisions = new();
    private void Awake()
    {
        var uiDocRoot = GetComponent<UIDocument>().rootVisualElement;
        this.list = uiDocRoot.Q<ListView>("decision-detail-list");
        list.makeItem = MakeItem;
        list.bindItem = BindItem;
    }

    private void BindItem(VisualElement element, int index)
    {
        if (element is GraphicsPanel content)
        {
            var decisionData = auxDecisions[index];
            content.ActionName.text = decisionData.actionName;
            var evaluatedConsiderations = decisionData.evaluatedConsiderations;
            var considerationCount = evaluatedConsiderations.Count;
            var curvesAndValues = new (Curve, float)[considerationCount];

            // what is this? names of what?
            string names = "";
            // what is this? values of what?
            string values = "";
            for (int i = 0; i < considerationCount; i++)
            {
                var consideration = evaluatedConsiderations[i];
                curvesAndValues[i] = (consideration.Curve, consideration.InputValue);

                string connector = (i != considerationCount - 1) ? " * " : "";
                names += "(" + consideration.EvaluatedVariableName + ")" + connector;
                values += consideration.UtilityValue.ToString("N3") + connector;
            }
            string totalUtility = decisionData.actionScore.ToString();
            // Plot the line that represents the total utility
            content.Chart.SetCurves(curvesAndValues, true);
            string priority = decisionData.priority.ToString("N3");
            string scaleFactor = decisionData.scaleFactor.ToString("N3");
            content.PriorityAction.text = $"Action priority (P): {priority}";
            content.ScaleFactor.text = $"Scale factor (SF): {scaleFactor}";
            content.BaseFormula.text = $"Utility formula: {names} * SF * P";
            content.FormulaUtility.text = $"Current values: ({values}) * {priority} * {scaleFactor}"; 
            content.TotalUtility.text = $"Total utility: {totalUtility}";
            content.DisplayEvaluatedConsiderations(evaluatedConsiderations);
        }
    }

    private VisualElement MakeItem()
    {
        return new GraphicsPanel();
    }

    internal void DisplayDecisionDetails(DecisionPackage decisions)
    {
        //Re-assemble the options (DecisionData) into a single list so its easier to
        //handle them using the ListView
        auxDecisions.Clear();
        auxDecisions.Add(decisions.bestOption);
        auxDecisions.AddRange(decisions.otherOptions);
        list.itemsSource = auxDecisions;
        list.RefreshItems();
    }

    public void DisplaySensorDetails(SensorPackage sensorPackage)
    {
        Debug.Log("DO SOME FUN!!");
    }
}
