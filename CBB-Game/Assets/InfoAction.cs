using CBB.Api;
using CBB.Lib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InfoAction : MonoBehaviour
{
    public infoDD prefDD;

    public Image background;
    public Text nameAgent;
    public Text nameAction;
    public Dropdown evaluatorDD;
    public GameObject evaluatorInputs;
    public List<infoDD> inputsDD = new List<infoDD>();
    public Dropdown curveDD;

    public _Agent agent;

    public void Init(Tuple<string,object> action,_Agent agent)
    {
        this.agent = agent;

        // Agent name
        this.nameAgent.text = agent.body.name;

        // Action name
        this.nameAction.text = action.Item1;

        // Evaluator parameter
        var inputs = agent.inputs;
        var evaluators = UtilityEvaluator.GetEvaluators();
        evaluatorDD.ClearOptions();
        var evalautorNames = evaluators.Select(e => e.GetType().ToString()).ToList();
        evaluatorDD.AddOptions(evalautorNames);
        evaluatorDD.onValueChanged.AddListener((v) => SetInputDD(v,agent));

        // Curves parameter
        var curves = Curve.GetCurves();
        curveDD.ClearOptions();
        var curveNames = curves.Select( c => c.GetType().ToString()).ToList();
        curveDD.AddOptions(curveNames);
    }

    private void ClearEvaluatorInputs()
    {
        var inputCount = evaluatorInputs.transform.childCount;
        for (int i = inputCount - 1; i >= 0; i--)
        {
            Destroy(evaluatorInputs.transform.GetChild(i).gameObject);
        }
    }

    private void SetInputDD(int index, _Agent agent)
    {
        ClearEvaluatorInputs();
        var evaluators = UtilityEvaluator.GetEvaluators();
        var evlauator =evaluators[index];
        var atts = Attribute.GetCustomAttributes(evlauator.GetType()).ToList();
        var att = atts.Find(a => a.GetType() == typeof(EvaluatorAttribute)) as EvaluatorAttribute;
        if (att == null)
        {
            Debug.Log("El evalaudor no implementa el attributo 'EvaluatorAttribute'");
            return;
        }

        var inputsNames = agent.inputs.Select(i=> i.Item1).ToList();

        var parametersNames = att.inputsNames;
        for (int i = 0; i < parametersNames.Length; i++)
        {
            var dd = Instantiate(prefDD, evaluatorInputs.transform);
            dd.textName.text = parametersNames[i];
            dd.DD.ClearOptions();
            dd.DD.AddOptions(inputsNames);
            inputsDD.Add(dd);
        }
    }

    public void Select(bool b)
    {
        background.color = b ? Color.green : Color.red;
    }

    public float GetEvaluatorValue()
    {
        var evaluators = UtilityEvaluator.GetEvaluators();
        var evlauator = evaluators[evaluatorDD.value];

        object[] inputsValue = new object[inputsDD.Count];
        for (int i = 0; i < inputsDD.Count; i++)
        {
            var index = inputsDD[i].DD.value;
            var input = agent.inputs[index];
            inputsValue[i] = input.Item2;
        }

        return evlauator.Evaluate(inputsValue);
    }

    public float GetCurveValue(float v)
    {
        var value = Mathf.Clamp01(v);
        var curves = Curve.GetCurves();
        var curve = curves[curveDD.value];
        
        return curve.Calc(value);
    }

    public float GetValue()
    {
        var evaluatorValue = GetEvaluatorValue();
        var curveValue = GetCurveValue(evaluatorValue);
        return curveValue;
    }
}
