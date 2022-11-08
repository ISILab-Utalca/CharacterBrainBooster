using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;
using CBB.Api;
using System;

namespace CBB.Api
{

    public class Agent : MonoBehaviour
    {
        private _Agent agent;

        // Start is called before the first frame update
        void Start()
        {
            agent = CosntructAgent();
            AgentObserver.Instance.AddAgent(this);
        }

        public void PerformAction()
        {

        }

        private void OnDestroy()
        {
            AgentObserver.Instance.RemoveAgent(this);
        }

        private void OnEnable()
        {
            AgentObserver.Instance.AddAgent(this);
        }

        private void OnDisable()
        {
            AgentObserver.Instance.RemoveAgent(this);
        }

        private _Agent CosntructAgent()
        {
            var agent = GetAgent();
            var inputs = GetInputs(agent);
            var actions = GetActions(agent);
            return new _Agent(agent, inputs, actions);
        }

        private List<Tuple<string, object>> GetInputs(object behaviour)
        {
            var inputs = new List<Tuple<string, object>>();

            var fields = behaviour.GetType().GetFields();
            foreach (var field in fields)
            {
                var atts = field.GetCustomAttributes();
                if (atts.Any(a => a.GetType() == typeof(UtilityInputAttribute)))
                {
                    var inp = new Tuple<string, object>(field.Name, field);
                    inputs.Add(inp);
                }
            }

            var props = behaviour.GetType().GetProperties();
            foreach (var prop in props)
            {
                var atts = prop.GetCustomAttributes();
                if (atts.Any(a => a.GetType() == typeof(UtilityInputAttribute)))
                {
                    var inp = new Tuple<string, object>(prop.Name, prop);
                    inputs.Add(inp);
                }
            }

            return inputs;
        }

        private List<Tuple<string, object>> GetActions(object behaviour)
        {
            var actions = new List<Tuple<string, object>>();
            var meths = behaviour.GetType().GetMethods();
            foreach (var meth in meths)
            {
                var atts = meth.GetCustomAttributes();
                if (atts.Any(a => a.GetType() == typeof(UtilityActionAttribute)))
                {
                    var met = new Tuple<string, object>(meth.Name, meth);
                    actions.Add(met);
                }
            }

            var events = behaviour.GetType().GetEvents();
            foreach (var evt in events)
            {
                var atts = evt.GetCustomAttributes();
                if (atts.Any(a => a.GetType() == typeof(UtilityActionAttribute)))
                {
                    var ev = new Tuple<string, object>(evt.Name, evt);
                    actions.Add(ev);
                }
            }

            return actions;
        }

        private MonoBehaviour GetAgent()
        {
            var behaviours = GetComponents<MonoBehaviour>();
            foreach (var bh in behaviours)
            {
                var atts = System.Attribute.GetCustomAttributes(bh.GetType()).ToList();
                if (atts.Any(a => a.GetType() == typeof(UtilityAgentAttribute)))
                {
                    return bh; // encuentra a el primer agente y termina
                }
            }
            return null;
        }

    }
}

public struct _Agent
{
    public MonoBehaviour agent;
    public List<Tuple<string, object>> inputs;
    public List<Tuple<string, object>> actions;

    public _Agent(MonoBehaviour agent, List<Tuple<string, object>> inputs, List<Tuple<string, object>> actions)
    {
        this.agent = agent;
        this.inputs = inputs;
        this.actions = actions;
    }
}