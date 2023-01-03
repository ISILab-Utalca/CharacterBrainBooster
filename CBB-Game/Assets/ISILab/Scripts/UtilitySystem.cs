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
            Debug.Log("<b><color=#d4fffeff>[CBB]</color>:</b> Load agent data.");
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

                agents.Add(new AgentData(type, inputs.ToList(), actions.ToList()));
            }

            return agents.ToArray();
        }

        public static Type[] CollectAgentTypes()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            var assemblies = currentDomain.GetAssemblies().ToArray();
            var types = new List<Type>();
            foreach (var assembly in assemblies)
            {
                var tys = assembly.GetTypes()
                           .Where(t => t.GetCustomAttributes(typeof(UtilityAgentAttribute), false).Length > 0)
                           .ToArray();
                types.AddRange(tys);
            }
            return types.ToArray();
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
                    var input = new Variable(att.Name, field.FieldType, typeOwner);
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
                    var input = new Variable(att.Name, prop.PropertyType, typeOwner);
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
                    var action = new ActionInfo(att.Name, type);
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
                    var action = new ActionInfo(att.Name, type);
                    actions.Add(action);
                }
            }

            return actions.ToArray();
        }

        public static ActionMetaInfo[] CollectActionMetaInfo(Type type)
        {
            var actms = new List<ActionMetaInfo>();
            var meths = type.GetMethods();
            foreach (var meth in meths)
            {
                var atts = meth.GetCustomAttributes();
                if (atts.Any(a => a.GetType() == typeof(UtilityActionAttribute)))
                {
                    var att = meth.GetCustomAttribute(typeof(UtilityActionAttribute), false) as UtilityActionAttribute;
                    var action = new ActionInfo(att.Name, type);
                    var actm = new ActionMetaInfo(action, meth, null, att);
                    actms.Add(actm);
                }
            }

            var events = type.GetEvents();
            foreach (var evt in events)
            {
                var atts = evt.GetCustomAttributes();
                if (atts.Any(a => a.GetType() == typeof(UtilityActionAttribute)))
                {
                    var att = evt.GetCustomAttribute(typeof(UtilityActionAttribute), false) as UtilityActionAttribute;
                    var action = new ActionInfo(att.Name, type);
                    var actm = new ActionMetaInfo(action, null, evt, att);
                    actms.Add(actm);
                }
            }

            return actms.ToArray();
        }
    }

    public class ActionMetaInfo // (!!) esta clase es para encapsular la info de una accion tanto su methodInfo o eventInfo, su actionAttribute y actionInfo
    {
        public ActionInfo actionInfo;
        public MethodInfo methodInfo;
        public EventInfo eventInfo;
        public UtilityActionAttribute atribute;

        public ActionMetaInfo(ActionInfo actionInfo, MethodInfo methodInfo, EventInfo eventInfo, UtilityActionAttribute atribute)
        {
            this.actionInfo = actionInfo;
            this.methodInfo = methodInfo;
            this.eventInfo = eventInfo;
            this.atribute = atribute;
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