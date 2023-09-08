using CBB.Api;
using CBB.Lib;
using System;
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
    private DropdownField actionField;
    private Toggle publicToggle;
    private VisualElement actionParameters;

    // Middle panel
    private DropdownField evaluatorDropdown;
    private VisualElement evaluatorParamaters;

    // Right panel
    private DropdownField curveDropdown;
    private Toggle invertedToggle;
    //private Chart chart;
    private VisualElement curveParameters;

    // internal
    private Type _self;
    private Type[] _others;
    private Type _sOther;

    // internal evaluator
    private UtilityEvaluator[] _evaluators;
    private int _eIndex;

    // internal curve
    private Curve[] _curves;
    private int _cIndex;

    // internal action
    private ActionInfo[] _actions;
    private int _aIndex;


    public UtilityPanel(AgentBrainData agent, Consideration consideration, Action OnChange)
    {
        var vt = Resources.Load<VisualTreeAsset>("UtilityPanel");
        vt.CloneTree(this);
        _evaluators = UtilityEvaluator.GetEvaluators().ToArray(); // (!) esta imeplementacion crea un objeto por cada 'Evalaudor' por cada utilityPanel lo cual puede sr inecesario
        _curves = Curve.GetCurves().ToArray(); // (!) esta imeplementacion crea un objeto por cada 'Curve' por cada utilityPanel lo cual puede sr inecesario
        //_self = agent.baseData.agentType;

        // AgentLabel 
        this.agentLabel = this.Q<Label>("AgentLabel");
        //this.agentLabel.text = (agent.baseData.agentType.GetCustomAttributes(typeof(UtilityAgentAttribute), false)[0] as UtilityAgentAttribute).Name;

        // OtherDropdown
        this.otherDropdown = this.Q<DropdownField>("OtherDropdown");
        _others = UtilitySystem.CollectAgentTypes();
        var choices = _others.Select(t => (t.GetCustomAttributes(typeof(UtilityAgentAttribute), false)[0] as UtilityAgentAttribute).Name);
        this.otherDropdown.choices = (choices.Concat(new List<string>() { "Nothing" })).ToList();
        this.otherDropdown.index = choices.ToList().Count;
        this.otherDropdown.RegisterCallback<ChangeEvent<string>>(e =>
        {
            _sOther = (this.otherDropdown.index == _others.Length) ? null : _others[otherDropdown.index];
            UpdateEvaluatorParameter(_evaluators[_eIndex]);
        });

        // DeleteButton
        // 14/07/2023 This button was commented because the AgentBrainData doesn't have a considerations property
        // anymore. Instead, the considerations are nested in the Actions attached to this brain
        //this.deleteButton = this.Q<Button>("DeleteButton");
        //this.deleteButton.clicked += () => { 
        //    agent.considerations.Remove(consideration);
        //    OnChange?.Invoke();
        //};

        // ConsiderationField (Left panel)
        this.nameField = this.Q<TextField>("NameField");
        this.nameField.value = consideration.name;
        this.nameField.RegisterCallback<ChangeEvent<string>>(e =>
        {
            consideration.name = e.newValue;
        });
        this.publicToggle = this.Q<Toggle>("PublicToggle");
        this.publicToggle.value = consideration.isPublic;
        this.publicToggle.RegisterCallback<ChangeEvent<bool>>(b =>
        {
            consideration.isPublic = b.newValue;
        });

        // ActionField
        this.actionField = this.Q<DropdownField>("ActionField");
        this.actionField.style.display = DisplayStyle.None;

        // ActionParameters
        this.actionParameters = this.Q<VisualElement>("ActionParameters");

        // EvaluatorParamenters
        this.evaluatorParamaters = this.Q<VisualElement>("EvaluatorParameters");

        // EvaluatorDropdown
        this.evaluatorDropdown = this.Q<DropdownField>("EvaluatorDropdown");
        this.evaluatorDropdown.choices = _evaluators.Select((e) =>
        {
            var att = e.GetType().GetCustomAttributes(typeof(EvaluatorAttribute), false)[0] as EvaluatorAttribute;
            return att.Name;
        }).ToList();
        _eIndex = _evaluators.ToList().FindIndex(e => e.GetType().Equals(consideration.evaluator.GetType()));
        this.evaluatorDropdown.index = _eIndex;
        this.evaluatorDropdown.RegisterCallback<ChangeEvent<string>>(e =>
        {
            _eIndex = this.evaluatorDropdown.index;
            consideration.evaluator = _evaluators[_eIndex];
            UpdateEvaluatorParameter(_evaluators[_eIndex]);
        });
        UpdateEvaluatorParameter(_evaluators[_eIndex]);

        // Curve
        _cIndex = _curves.ToList().FindIndex(c => c.GetType().Equals(consideration.curve.GetType()));

        // CurveParameters
        this.curveParameters = this.Q<VisualElement>("CurveParameters");
        this.UpdateCurveParamter(_curves[_cIndex]);

        // Chart
        // this.chart = this.Q<Chart>("Chart");
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
        this.curveDropdown.choices = _curves.Select((c) =>
        {
            var att = c.GetType().GetCustomAttributes(typeof(CurveAttribute), false)[0] as CurveAttribute;
            return att.Name;
        }).ToList();
        this.curveDropdown.index = _curves.ToList().FindIndex(c => c.GetType().Equals(consideration.curve.GetType()));
        this.curveDropdown.RegisterCallback<ChangeEvent<string>>(e =>
        {
            _cIndex = this.curveDropdown.index;
            consideration.curve = _curves[_cIndex];
            UpdateCurveParamter(_curves[_cIndex]);
            UpdateChart(_curves[_cIndex]);
        });

    }

    public UtilityPanel(AgentBrainData agent, ActionUtility action, Action OnChange)
    {
        // Init basic info
        var vt = Resources.Load<VisualTreeAsset>("UtilityPanel");
        vt.CloneTree(this);
        _evaluators = UtilityEvaluator.GetEvaluators().ToArray(); // (!) esta imeplementacion crea un objeto por cada 'Evalaudor' por cada utilityPanel lo cual puede sr inecesario
        _curves = Curve.GetCurves().ToArray(); // (!) esta imeplementacion crea un objeto por cada 'Curve' por cada utilityPanel lo cual puede sr inecesario
        //_self = agent.baseData.agentType;
        _actions = UtilitySystem.CollectActions(_self);

        // AgentLabel 
        this.agentLabel = this.Q<Label>("AgentLabel");
        //this.agentLabel.text = (agent.baseData.agentType.GetCustomAttributes(typeof(UtilityAgentAttribute), false)[0] as UtilityAgentAttribute).Name;

        // OtherDropdown
        this.otherDropdown = this.Q<DropdownField>("OtherDropdown");
        this.otherDropdown.style.display = DisplayStyle.None;

        // DeleteButton
        this.deleteButton = this.Q<Button>("DeleteButton");
        this.deleteButton.clicked += () =>
        {
            //agent.actions.Remove(action);
            OnChange?.Invoke();
        };

        // ConsiderationField (Left panel)
        this.nameField = this.Q<TextField>("NameField");
        this.nameField.value = action.name;
        this.nameField.RegisterCallback<ChangeEvent<string>>(e =>
        {
            action.name = e.newValue;
        });
        this.publicToggle = this.Q<Toggle>("PublicToggle");
        this.publicToggle.style.display = DisplayStyle.None;

        // ActionField
        this.actionField = this.Q<DropdownField>("ActionField");
        this.actionField.choices = _actions.Select(a => a.name).ToList();
        this.actionField.index = _actions.ToList().FindIndex(a => a.name == action.actionInfo.name);
        this.actionField.RegisterCallback<ChangeEvent<string>>(a =>
        {

        });
        //UpdateActionParameter(); // (!!!) implementar

        // ActionParameters
        this.actionParameters = this.Q<VisualElement>("ActionParameters");
        UpdateActionParameter(action);

        // EvaluatorParamenters
        this.evaluatorParamaters = this.Q<VisualElement>("EvaluatorParameters");

        // EvaluatorDropdown
        this.evaluatorDropdown = this.Q<DropdownField>("EvaluatorDropdown");
        this.evaluatorDropdown.choices = _evaluators.Select((e) =>
        {
            var att = e.GetType().GetCustomAttributes(typeof(EvaluatorAttribute), false)[0] as EvaluatorAttribute;
            return att.Name;
        }).ToList();
        _eIndex = _evaluators.ToList().FindIndex(e => e.GetType().Equals(action.evaluator.GetType()));
        this.evaluatorDropdown.index = _eIndex;
        this.evaluatorDropdown.RegisterCallback<ChangeEvent<string>>(e =>
        {
            _eIndex = this.evaluatorDropdown.index;
            action.evaluator = _evaluators[_eIndex];
            UpdateEvaluatorParameter(_evaluators[_eIndex]);
        });
        UpdateEvaluatorParameter(_evaluators[_eIndex]);

        // Curve
        _cIndex = _curves.ToList().FindIndex(c => c.GetType().Equals(action.curve.GetType()));

        // CurveParameters
        this.curveParameters = this.Q<VisualElement>("CurveParameters");
        this.UpdateCurveParamter(_curves[_cIndex]);

        // Chart
        //this.chart = this.Q<Chart>("Chart");
        this.UpdateChart(_curves[_cIndex]);

        // InvertedToggle
        this.invertedToggle = this.Q<Toggle>("InvertedToggle");
        var curve = _curves.ToList().Find(c => c.GetType().Equals(action.curve.GetType())).Inverted;
        this.invertedToggle.value = curve;
        this.invertedToggle.RegisterCallback<ChangeEvent<bool>>(e =>
        {
            _curves[_cIndex].Inverted = e.newValue;
            UpdateChart(_curves[_cIndex]);
        });

        // CurveDropdown
        this.curveDropdown = this.Q<DropdownField>("CurveDropdown");
        this.curveDropdown.choices = _curves.Select((c) =>
        {
            var att = c.GetType().GetCustomAttributes(typeof(CurveAttribute), false)[0] as CurveAttribute;
            return att.Name;
        }).ToList();
        this.curveDropdown.index = _curves.ToList().FindIndex(c => c.GetType().Equals(action.curve.GetType()));
        this.curveDropdown.RegisterCallback<ChangeEvent<string>>(e =>
        {
            _cIndex = this.curveDropdown.index;
            action.curve = _curves[_cIndex];
            UpdateCurveParamter(_curves[_cIndex]);
            UpdateChart(_curves[_cIndex]);
        });
    }

    private void UpdateActionParameter(ActionUtility action)
    {
        this.actionParameters.Clear();

        var metaInfos = UtilitySystem.CollectActionMetaInfo(action.actionInfo.ownerType);
        var ai = metaInfos.First(mi => mi.actionInfo.name == action.actionInfo.name);   // (!!) el ACTION QUE RECIVE POR PARAMETROS NUNCA SERA EL MISMO QUE SE BUSCA EN EL METAINFO  

        if (ai.methodInfo != null)
        {
            var meth = ai.methodInfo;
            var att = ai.atribute;
            var parms = meth.GetParameters();
            for (int i = 0; i < parms.Length; i++)
            {
                var dropdown = new DropdownField();
                dropdown.label = att.Inputs[i];
                //parms[i].ParameterType; // (!!) esto tengo que usarlo para mostrar los valores en el dropdown que sean del tipo correcto
                actionParameters.Add(dropdown);
            }
        }
        else if (ai.eventInfo != null)
        {
            var evnt = ai.eventInfo;
            // (!!) falta implementar los inputs de los eventos 
        }

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
        var parms = p.Where(f => f.GetCustomAttributes(typeof(ParamAttribute), false).Count() > 0).ToList();
        for (int i = 0; i < parms.Count; i++)
        {
            var att = (parms[i].GetCustomAttributes(typeof(ParamAttribute), false)[0] as ParamAttribute);
            var parm = parms[i];
            var sliderField = new NumberSliderField(att.Name, (float)parm.GetValue(curve), att.Min, att.Max, (v) =>
            {
                ;
                parm.SetValue(curve, v);
                UpdateChart(curve);
            }); ;
            curveParameters.Add(sliderField);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="curve"></param>
    private void UpdateChart(Curve curve)
    {
        //chart.SetCurve(curve);
    }
}
