using CBB.Api;
using CBB.Lib;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class UtilityPanel : VisualElement
{
    private TextField nameField;
    private VisualElement actionParameters;

    private DropdownField evaluatorDropdown;
    private VisualElement evaluatorParamaters;

    private DropdownField curveDropdown;
    private Toggle invertedToggle;
    private Chart chart;
    private VisualElement curveParameters;

    private UtilityEvaluator[] _evaluators;
    private Curve[] _curves;
    private int _cIndex;

    public UtilityPanel(AgentData.Consideration consideration)
    {
        var vt = Resources.Load<VisualTreeAsset>("UtilityPanel");
        vt.CloneTree(this);
        _evaluators = UtilityEvaluator.GetEvaluators().ToArray();
        _curves = Curve.GetCurves().ToArray();

        // ConsiderationField
        this.nameField = this.Q<TextField>("NameField"); 
        this.nameField.value = consideration.name;
        this.nameField.RegisterCallback<ChangeEvent<string>>(e => {
            consideration.name = e.newValue;
        });

        // ActionParameters
        this.actionParameters = this.Q<VisualElement>("ActionParameters");

        // EvaluatorParamenters
        this.evaluatorParamaters = this.Q<VisualElement>("EvaluatorParameters");

        // EvaluatorDropdown
        this.evaluatorDropdown = this.Q<DropdownField>("EvaluatorDropdown");
        this.evaluatorDropdown.choices = _evaluators.Select((e) => {
            var att = e.GetType().GetCustomAttributes(typeof(EvaluatorAttribute), false)[0] as EvaluatorAttribute;
            return att.Name;
        }).ToList();
        this.evaluatorDropdown.index = _evaluators.ToList().FindIndex(e => e.GetType().Equals(consideration.evaluator.GetType())); // (!) no tiene que partir en o sino que la que estaba guardada
        this.evaluatorDropdown.RegisterCallback<ChangeEvent<string>>(e => {
            var index = this.evaluatorDropdown.index;
            consideration.evaluator = _evaluators[index].GetType();
            UpdateEvaluatorParameter(_evaluators[index]);
        });

        // CurveParameters
        this.curveParameters = this.Q<VisualElement>("CurveParameters");

        // Chart
        this.chart = this.Q<Chart>("Chart");
        _cIndex = _curves.ToList().FindIndex(c => c.GetType().Equals(consideration.curve.GetType()));
        Debug.Log(_cIndex);
        UpdateChart(_curves[_cIndex]);

        // InvertedToggle
        this.invertedToggle = this.Q<Toggle>("InvertedToggle");
        var curve = _curves.ToList().Find(c => c.GetType().Equals(consideration.curve.GetType())).Inverted;
        this.invertedToggle.value = curve;
        this.invertedToggle.RegisterCallback<ChangeEvent<bool>>(e =>
        {
            _curves[_cIndex].Inverted = e.newValue;
            UpdateChart(_curves[_cIndex]);
        });

        // CurveDropdown
        this.curveDropdown = this.Q<DropdownField>("CurveDropdown");
        this.curveDropdown.choices = _curves.Select((c) => {
            var att = c.GetType().GetCustomAttributes(typeof(CurveAttribute), false)[0] as CurveAttribute;
            return att.Name;
        }).ToList();
        this.curveDropdown.index = _curves.ToList().FindIndex(c => c.GetType().Equals(consideration.curve.GetType()));
        this.curveDropdown.RegisterCallback<ChangeEvent<string>>(e => {
            _cIndex = this.curveDropdown.index;
            consideration.curve = _curves[_cIndex];
            UpdateCurveParamter(_curves[_cIndex]);
            UpdateChart(_curves[_cIndex]);
        });

    }

    private void UpdateEvaluatorParameter(UtilityEvaluator evaluator)
    {
        this.evaluatorParamaters.Clear();
        var att = evaluator.GetType().GetCustomAttributes(typeof(EvaluatorAttribute), false)[0] as EvaluatorAttribute;
        for (int i = 0; i < att.inputsNames.Length; i++)
        {
            var dropdown = new DropdownField();
            dropdown.label = att.inputsNames[i];
            evaluatorParamaters.Add(dropdown);
        }
    }

    private void UpdateCurveParamter(Curve curve)
    {
        this.curveParameters.Clear();
        var att = curve.GetType().GetCustomAttributes(typeof(CurveAttribute), false)[0] as CurveAttribute;
        for (int i = 0; i < att.parms.Length; i++)
        {
            var dropdown = new DropdownField();
            dropdown.label = att.parms[i];
            dropdown.RegisterCallback<ChangeEvent<string>>(e => {
                UpdateChart(curve);
            });
            curveParameters.Add(dropdown);
        }
    }

    private void UpdateChart(Curve curve)
    {
        chart.SetCurve(curve);
    }
}
