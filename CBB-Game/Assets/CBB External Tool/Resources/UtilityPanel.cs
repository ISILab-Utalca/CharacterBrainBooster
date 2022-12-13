using CBB.Api;
using CBB.Lib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class UtilityPanel : VisualElement
{
    // Top panel
    private Label agentLabel;
    private DropdownField otherDropdown;
    private Button deleteButton;

    // Left panel
    private TextField nameField;
    private VisualElement actionParameters;

    // Middle panel
    private DropdownField evaluatorDropdown;
    private VisualElement evaluatorParamaters;

    // Right panel
    private DropdownField curveDropdown;
    private Toggle invertedToggle;
    private Chart chart;
    private VisualElement curveParameters;

    private UtilityEvaluator[] _evaluators;
    private Type _self;
    private Curve[] _curves;
    private int _cIndex;
    private Type[] _others;
    private Type _sOther; 

    public UtilityPanel(AgentData agent, Consideration consideration, Action OnChange)
    {
        var vt = Resources.Load<VisualTreeAsset>("UtilityPanel");
        vt.CloneTree(this);
        _evaluators = UtilityEvaluator.GetEvaluators().ToArray();
        _curves = Curve.GetCurves().ToArray();
        _self = agent.type;

        // AgentLabel 
        this.agentLabel = this.Q<Label>("AgentLabel");
        this.agentLabel.text = (agent.type.GetCustomAttributes(typeof(UtilityAgentAttribute), false)[0] as UtilityAgentAttribute).Name;

        // OtherDropdown
        this.otherDropdown = this.Q<DropdownField>("OtherDropdown");
        _others = UtilitySystem.CollectAgentTypes();
        var choices = _others.Select(t => (t.GetCustomAttributes(typeof(UtilityAgentAttribute), false)[0] as UtilityAgentAttribute).Name);
        this.otherDropdown.choices = (choices.Concat(new List<string>() { "Nothing" })).ToList();
        this.otherDropdown.index = choices.ToList().Count;
        this.otherDropdown.RegisterCallback<ChangeEvent<string>>(e => {
            if(this.otherDropdown.index == _others.Length)
            {
                _sOther = null;
            }
            else
            {
                _sOther = _others[otherDropdown.index];
            }
        });


        // DeleteButton
        this.deleteButton = this.Q<Button>("DeleteButton");
        this.deleteButton.clicked += () => { 
            agent.considerations.Remove(consideration);
            OnChange?.Invoke();
        };

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

        // Curve
        _cIndex = _curves.ToList().FindIndex(c => c.GetType().Equals(consideration.curve.GetType()));

        // CurveParameters
        this.curveParameters = this.Q<VisualElement>("CurveParameters");
        this.UpdateCurveParamter(_curves[_cIndex]);

        // Chart
        this.chart = this.Q<Chart>("Chart");
        this.UpdateChart(_curves[_cIndex]);

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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="evaluator"></param>
    private void UpdateEvaluatorParameter(UtilityEvaluator evaluator)
    {
        var allowedTypes = (evaluator.GetType().GetCustomAttributes(typeof(ParamsAllowedAttribute), false)[0] as ParamsAllowedAttribute).Parms;
        var selfInputs = UtilitySystem.CollectVariables(_self, allowedTypes);
        var otherInputs = new Variable[0];
        if (_sOther != null)
        {
            otherInputs = UtilitySystem.CollectVariables(_sOther, allowedTypes);
        }
        var globalVaribles = Globals.globalVariables.Where(gv => allowedTypes.Contains(gv.type));
        var choices = new string[0]
            .Concat(selfInputs.Select(s => s.name))
            .Concat(otherInputs.Select(o => "(other) " + o.name))
            .Concat(globalVaribles.Select(g => "(global min) " + g.name))
            .Concat(globalVaribles.Select(g => "(global max) " + g.name));

        this.evaluatorParamaters.Clear();
        var att = evaluator.GetType().GetCustomAttributes(typeof(EvaluatorAttribute), false)[0] as EvaluatorAttribute;
        for (int i = 0; i < att.inputsNames.Length; i++)
        {
            var dropdown = new DropdownField();
            dropdown.label = att.inputsNames[i];
            dropdown.choices.AddRange(choices);
            evaluatorParamaters.Add(dropdown);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="curve"></param>
    private void UpdateCurveParamter(Curve curve)
    {
        this.curveParameters.Clear();
        var p = curve.GetType().GetFields();
        var parms = p.Where( f => f.GetCustomAttributes(typeof(ParamAttribute), false).Count() > 0).ToList();
        for (int i = 0; i < parms.Count; i++)
        {
            var att = (parms[i].GetCustomAttributes(typeof(ParamAttribute), false)[0] as ParamAttribute);
            var parm = parms[i];
            var sliderField = new NumberSliderField(att.Name, (float)parm.GetValue(curve), att.Min, att.Max, (v) => {;
                parm.SetValue(curve, v);
                UpdateChart(curve);
            });;
            curveParameters.Add(sliderField);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="curve"></param>
    private void UpdateChart(Curve curve)
    {
        chart.SetCurve(curve);
    }
}
