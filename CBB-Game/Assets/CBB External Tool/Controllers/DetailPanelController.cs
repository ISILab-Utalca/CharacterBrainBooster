using CBB.Lib;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class DetailPanelController : MonoBehaviour
{
    readonly JsonSerializerSettings settings = new()
    {
        NullValueHandling = NullValueHandling.Include,
        MissingMemberHandling = MissingMemberHandling.Error,
        TypeNameHandling = TypeNameHandling.Auto,
    };
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
            for(int i = 0; i < considerationCount; i++)
            {
                var consideration = evaluatedConsiderations[i];
                curvesAndValues[i] = (consideration.Curve, consideration.InputValue);
            }
            var totalUtility = decisionData.actionScore.ToString();
            content.Chart.SetCurves(curvesAndValues);
            content.TotalUtility.text = "Total utility: " + totalUtility;
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
}
