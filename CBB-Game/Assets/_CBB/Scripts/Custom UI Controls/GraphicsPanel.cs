using CBB.ExternalTool;
using CBB.Lib;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class GraphicsPanel : VisualElement
{
    #region FACTORY
    public new class UxmlFactory : UxmlFactory<GraphicsPanel, UxmlTraits> { }
    #endregion
    #region PROPERTIES
    public Foldout ActionName { get; set; }

    public Label BaseFormula { get; set; }
    public Label FormulaUtility { get; set; }
    public Label PriorityAction { get; set; }
    public Label ScaleFactor { get; set; }
    public Label TotalUtility { get; set; }
    public Chart Chart { get; set; }

    private ListView evaluationsList;
    private List<ConsiderationData> aux_considerations;
    #endregion
    public GraphicsPanel()
    {
        var visualTree = Resources.Load<VisualTreeAsset>("GraphicsPanel");
        visualTree.CloneTree(this);

        this.ActionName = this.Q<Foldout>("Title");
        this.Chart = this.Q<Chart>();
        this.evaluationsList = this.Q<ListView>("evaluations-list");
        this.BaseFormula = this.Q<Label>("base-formula-utility");
        this.ScaleFactor = this.Q<Label>("scale-factor");
        this.FormulaUtility = this.Q<Label>("formula-utility-score");
        this.PriorityAction = this.Q<Label>("priority-action");
        this.TotalUtility = this.Q<Label>("total-utility-score");

        evaluationsList.makeItem = MakeItem;
        evaluationsList.bindItem = BindItem;
        evaluationsList.itemsSource = aux_considerations;
    }

    private void BindItem(VisualElement element, int index)
    {
        if (element is ConsiderationView cv)
        {
            cv.ShowConsideration(aux_considerations[index]);
        }
    }

    private VisualElement MakeItem()
    {
        return new ConsiderationView();
    }

    public void DisplayEvaluatedConsiderations(List<ConsiderationData> evaluatedConsiderations)
    {
        aux_considerations = evaluatedConsiderations;
        evaluationsList.itemsSource = aux_considerations;
        evaluationsList.RefreshItems();
    }
}
