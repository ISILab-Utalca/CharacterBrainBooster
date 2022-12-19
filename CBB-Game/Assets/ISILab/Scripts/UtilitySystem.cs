using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CBB.Api
{

    public static class UtilitySystem
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeSceneLoadRuntimeMethod()
        {
            Debug.Log("Before scene loaded");
            CollectAgentBaseData();
        }

        public static AgentData[] CollectAgentBaseData()
        {
            var agentTypes = UtilitySystem.CollectAgentTypes();
            var agents = new List<AgentData>();
            foreach (var type in agentTypes)
            {
                var inputs = UtilitySystem.CollectVariables(type);
                var actions = UtilitySystem.CollectActions(type);

                agents.Add(new AgentData(inputs.ToList(), actions.ToList()));
            }

            return agents.ToArray();
        }

        public static Type[] CollectAgentTypes()
        {
            var types = Assembly.GetExecutingAssembly()
                           .GetTypes()
                           .Where(t => t.GetCustomAttributes(typeof(UtilityAgentAttribute), false).Length > 0)
                           .ToArray();
            return types;
        }

        public static Variable[] CollectVariables(Type typeOwner)
        {
            var inputs = new List<Variable>();
            var fields = typeOwner.GetFields();
            foreach (var field in fields)
            {
                var atts = field.GetCustomAttributes();
                if (atts.Any(a => a.GetType() == typeof(UtilityInputAttribute)))
                {
                    var att = field.GetCustomAttributes(typeof(UtilityInputAttribute), false)[0] as UtilityInputAttribute;
                    var input = new Variable(att.Name, field.FieldType, typeOwner, null);
                    inputs.Add(input);
                }
            }

            var props = typeOwner.GetProperties();
            foreach (var prop in props)
            {
                var atts = prop.GetCustomAttributes();
                if (atts.Any(a => a.GetType() == typeof(UtilityInputAttribute)))
                {
                    var att = prop.GetCustomAttributes(typeof(UtilityInputAttribute), false)[0] as UtilityInputAttribute;
                    var input = new Variable(att.Name, prop.PropertyType, typeOwner, null);
                    inputs.Add(input);
                }
            }
            return inputs.ToArray();
        }

        public static Variable[] CollectVariables(Type typeOwner,Type variable)
        {
            var inputs = CollectVariables(typeOwner).Where(v => v.type == variable);
            return inputs.ToArray();
        }

        public static Variable[] CollectVariables(Type typeOwner, Type[] variable)
        {
            var vs = CollectVariables(typeOwner);
            var inputs = vs.Where(v => variable.Contains(v.type));
            return inputs.ToArray();
        }

        public static ActionInfo[] CollectActions(Type type)
        {
            var actions = new List<ActionInfo>();
            var meths = type.GetMethods();
            foreach (var meth in meths)
            {
                var atts = meth.GetCustomAttributes();
                if (atts.Any(a => a.GetType() == typeof(UtilityActionAttribute)))
                {
                    var att = meth.GetCustomAttribute(typeof(UtilityActionAttribute), false) as UtilityActionAttribute;
                    var action = new ActionInfo(att.Name, type, null);
                    actions.Add(action);
                }
            }

            var events = type.GetEvents();
            foreach (var evt in events)
            {
                var atts = evt.GetCustomAttributes();
                if (atts.Any(a => a.GetType() == typeof(UtilityActionAttribute)))
                {
                    var att = evt.GetCustomAttribute(typeof(UtilityActionAttribute), false) as UtilityActionAttribute;
                    var action = new ActionInfo(att.Name, type, null);
                    actions.Add(action);
                }
            }

            return actions.ToArray();
        }
    }

    public class AgentObserver
    {
        private static AgentObserver instance;
        public List<AgentBeahaviour> Agents = new List<AgentBeahaviour>();

        public static AgentObserver Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AgentObserver();
                }
                return instance;
            }
        }

        public void AddAgent(AgentBeahaviour agent)
        {
            Agents.Add(agent);
        }

        public void RemoveAgent(AgentBeahaviour agent)
        {
            Agents.Remove(agent);
        }
    }
}