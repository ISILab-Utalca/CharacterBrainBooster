using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;
using CBB.Api;
using System;
using CBB.Lib;

namespace CBB.Api
{

    public class Agent : MonoBehaviour // brain
    {
        private _Agent agent;

        private float lastTime = 0;
        private float cooldown = 50; //ms o seg ?

        private List<Utility> utilities = new List<Utility>();

        public _Agent Data => agent;

        private void Awake()
        {
            agent = CosntructAgent();
            AgentObserver.Instance.AddAgent(this);
        }

        void Start()
        {
            // esto no deberia estar aqui pero cumple para el propotipo
            utilities = new CFCALIV().GetUtilities(agent);
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

        public Action GetAction()
        {
            //agent.actions
            return null;
        }

        public bool IsAvailable()
        {
            if((Time.time - lastTime) > cooldown)
            {
                lastTime = Time.time;
                return true;
            }
            return false;
        }

        private _Agent CosntructAgent()
        {
            var agent = GetAgent();
            var inputs = GetInputs(agent);
            var actions = GetActions(agent);
            return new _Agent(this,agent, inputs, actions);
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

public class _Agent
{
    public Agent brain;
    public MonoBehaviour body;
    public List<Tuple<string, object>> inputs;
    public List<Tuple<string, object>> actions;

    public _Agent(Agent brain, MonoBehaviour body, List<Tuple<string, object>> inputs, List<Tuple<string, object>> actions)
    {
        this.brain = brain;
        this.body = body;
        this.inputs = inputs;
        this.actions = actions;
    }

    public T GetInput<T>(string name)
    {
        var v = inputs.First(i => i.Item1.Equals(name));
        return (T)v.Item2;
    }

    public object GetAction(string name)
    {
        var v = actions.First(a => a.Item1.Equals(name));
        return v.Item2;
    }
}