using CBB.Api;
using CBB.Lib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Obsolete]
public class CFCALIV // hace lo que la interfaz
{
    public List<Utility> GetUtilities(_Agent agent)
    {
        var utilities = new List<Utility>();

        foreach (var action in agent.actions)
        {
            if (action.Item1.Equals("Chase"))
            {
                var u = GenerateUtilitiesChase(agent, action.Item2);
                utilities.Concat(u);

            }
            else if (action.Item1.Equals("Run"))
            {
                var u = GenerateUtilitiesRun(agent, action.Item2);
                utilities.Concat(u);
            }
            else if (action.Item1.Equals("Stop"))
            {
                var curve = new Linear(); //.Calc(0.5f);
                var evaluator = new Identity();
                var u = new Utility(agent.brain ,evaluator, curve, action.Item2);
                utilities.Add(u);
            }
        }
        return utilities;
    }

    public List<Utility> GenerateUtilitiesChase(_Agent agent, object act)
    {
        var toReturn = new List<Utility>(); 
        var agents = AgentObserver.Instance.Agents;
        foreach (var other in agents)
        {
            if (agent.body == other)
                continue;

            var otherInputs = GetOtherInputs(agent, other.Data);
            var evaluator = new Divide(); //.Evaluate(valentia, 1f / dist); <= valentia / dist;
            var curve = new ExponencialInvertida();

            var utility = new Utility(agent.brain ,evaluator, curve, act, otherInputs);
            toReturn.Add(utility);
        }

        return toReturn;
    }

    public List<Utility> GenerateUtilitiesRun(_Agent agent, object act)
    {
        var toReturn = new List<Utility>();
        var agents = AgentObserver.Instance.Agents;
        foreach (var other in agents)
        {
            if (agent.body == other)
                continue;

            var otherInputs = GetOtherInputs(agent,other.Data);
            var evaluator = new Multiply(); //.Evaluate(valentia, dist); <= valentia * dist
            var curve = new ExponencialInvertida();

            var utility = new Utility(agent.brain, evaluator, curve, act, otherInputs);
            toReturn.Add(utility);
        }

        return toReturn;
    }

    public List<Tuple<string, object>> GetOtherInputs(_Agent self, _Agent other)
    {
        var toReturn = new List<Tuple<string, object>>();

        toReturn.Concat(other.inputs);

        // estas se debererian tomar de algun lado en vez de crearse aqui (nose) 
        toReturn.Add(new Tuple<string, object>("Valentia", GetValentia(self, other)));
        toReturn.Add(new Tuple<string, object>("Distance", GetDist(self, other)));

        return toReturn;
    }

    public float GetDist(_Agent self,_Agent other)
    {
        var otherPos = other.GetInput<Vector3>("Position");
        var selfPos = self.GetInput<Vector3>("Position");
        return Vector3.Distance(otherPos, selfPos);
    }

    public float GetValentia(_Agent self,_Agent other)
    {
        var otherSize = other.GetInput<float>("Size");
        var maxSize = GetBigestAgentSize();
        var minSize = GetSmallestAgentSize();
        var selfSize = self.GetInput<float>("Size");
        return new Valentia().Evaluate(selfSize, otherSize, maxSize, minSize);
    }

    public float GetBigestAgentSize()
    {
        var agents = AgentObserver.Instance.Agents;
        return agents.Max(a => a.Data.GetInput<float>("Size"));
    }

    public float GetSmallestAgentSize()
    {
        var agents = AgentObserver.Instance.Agents;
        return agents.Min(a => a.Data.GetInput<float>("Size"));
    }
}
