using ArtificialIntelligence.Utility;
using CBB.Lib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Windows;

namespace CBB.Api
{
    /// <summary>
    /// Static class that provides various functions for collecting and storing data about agents and their variables and actions in the utility system.
    /// </summary>
    public static class UtilitySystem
    {
        //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        //private static void OnBeforeSceneLoadRuntimeMethod()
        //{
        //    Debug.Log("<b><color=#d4fffeff>[CBB]</color>:</b> Load agent data.");
        //    // We need to send this data over to the external CBB
        //    CollectAgentBaseData();
        //}

        /// <summary>
        /// Collects basic data about all agents in the utility system.
        /// </summary>
        /// <returns>Array of agent data for each agent in the utility system.</returns>
        //public static AgentData[] CollectAgentBaseData()
        //{
        //    var agentTypes = UtilitySystem.CollectAgentTypes();
        //    var agents = new List<AgentData>();
        //    foreach (var type in agentTypes)
        //    {
        //        var inputs = UtilitySystem.CollectVariables(type);
        //        var actions = UtilitySystem.CollectActions(type);

        //        agents.Add(new AgentData(type, inputs.ToList(), actions.ToList()));
        //    }

        //    return agents.ToArray();
        //}

        /// <summary>
        /// Collects the types of agents in the utility system.
        /// </summary>
        /// <returns>Array of agent types in the utility system.</returns>
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
        /// <summary>
        /// Collects the fields and properties that are part of its configuration.
        /// </summary>
        /// <param name="sensorInstance">The specific sensor instance to inspect</param>
        /// <returns>The configuration dictionary.</returns>
        public static Dictionary<string, object> CollectSensorConfiguration(Sensor sensorInstance)
        {
            Dictionary<string, object> sensorConfig = new();
            var sensorFields = sensorInstance.GetType().GetFields(BindingFlags.Instance);
            foreach (var field in sensorFields)
            {
                var atts = field.GetCustomAttributes();
                if (atts.Any(a => a.GetType() == typeof(SensorConfigurationAttribute)))
                {
                    string fieldName = field.Name;
                    object fieldValue = field.GetValue(sensorInstance);
                    sensorConfig.Add(fieldName, fieldValue);
                }
            }
            var sensorProperties = sensorInstance.GetType().GetProperties(BindingFlags.Instance);
            foreach (var property in sensorProperties)
            {
                var atts = property.GetCustomAttributes();
                if (atts.Any(a => a.GetType() == typeof(SensorConfigurationAttribute)))
                {
                    string propertyName = property.Name;
                    object propertyValue = property.GetValue(sensorInstance);
                    sensorConfig.Add(propertyName, propertyValue);
                }
            }
            return sensorConfig;
        }
        /// <summary>
        /// Collects fields and properties that are part of a sensor's individual memory
        /// </summary>
        /// <param name="sensorInstance">The specific sensor instance to inspect</param>
        /// <returns>The configuration dictionary.</returns>
        public static Dictionary<string, object> CollectSensorMemory(Sensor sensorInstance)
        {
            Dictionary<string, object> sensorConfig = new();
            var sensorFields = sensorInstance.GetType().GetFields(BindingFlags.Instance);
            foreach (var field in sensorFields)
            {
                var atts = field.GetCustomAttributes();
                if (atts.Any(a => a.GetType() == typeof(SensorMemoryAttribute)))
                {
                    string fieldName = field.Name;
                    object fieldValue = field.GetValue(sensorInstance);
                    sensorConfig.Add(fieldName, fieldValue);
                }
            }
            var sensorProperties = sensorInstance.GetType().GetProperties(BindingFlags.Instance);
            foreach (var property in sensorProperties)
            {
                var atts = property.GetCustomAttributes();
                if (atts.Any(a => a.GetType() == typeof(SensorMemoryAttribute)))
                {
                    string propertyName = property.Name;
                    object propertyValue = property.GetValue(sensorInstance);
                    sensorConfig.Add(propertyName, propertyValue);
                }
            }
            return sensorConfig;
        }
        /// <summary>
        /// Collects fields and properties that are part of a agent's internal state
        /// </summary>
        /// <param name="sensorInstance">The specific agent instance to inspect</param>
        /// <returns>The internal state represented by a list of its fields/properties</returns>
        public static List<AgentStateVariable> CollectAgentInternalState(IAgent agent)
        {
            List<AgentStateVariable> agentState = new();
            var agentFields = agent.GetType().GetFields(BindingFlags.Instance);
            foreach (var field in agentFields)
            {
                var atts = field.GetCustomAttributes();
                if (atts.Any(a => a.GetType() == typeof(AgentInternalStateAttribute)))
                {
                    agentState.Add(new AgentStateVariable
                    {
                        variableType = field.GetType(),
                        variableName = field.Name,
                        value = field.GetValue(agent)
                    });
                }
            }
            var agentProperties = agent.GetType().GetProperties(BindingFlags.Instance);
            foreach (var property in agentProperties)
            {
                var atts = property.GetCustomAttributes();
                if (atts.Any(a => a.GetType() == typeof(AgentInternalStateAttribute)))
                {
                    agentState.Add(new AgentStateVariable
                    {
                        variableType = property.GetType(),
                        variableName = property.Name,
                        value = property.GetValue(agent)
                    });
                }
            }
            return agentState;
        }
        /// <summary>
        /// Collects variables for a specific agent type.
        /// </summary>
        /// <param name="typeOwner">Type of the agent to collect variables for.</param>
        /// <returns>Array of variables for the specified agent type.</returns>
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

        /// <summary>
        /// Collects variables of a specific type for a specific agent type.
        /// </summary>
        /// <param name="typeOwner">Type of the agent to collect variables for.</param>
        /// <param name="variable">Type of variables to collect.</param>
        /// <returns>Array of variables of the specified type for the specified agent type.</returns>
        public static Variable[] CollectVariables(Type typeOwner, Type variable)
        {
            var inputs = CollectVariables(typeOwner).Where(v => v.type == variable);
            return inputs.ToArray();
        }

        /// <summary>
        /// Collects variables of specific types for a specific agent type.
        /// </summary>
        /// <param name="typeOwner">Type of the agent to collect variables for.</param>
        /// <param name="variable">Types of variables to collect.</param>
        /// <returns>Array of variables of the specified types for the specified agent type.</returns>
        public static Variable[] CollectVariables(Type typeOwner, Type[] variable)
        {
            var vs = CollectVariables(typeOwner);
            var inputs = vs.Where(v => variable.Contains(v.type));
            return inputs.ToArray();
        }

        /// <summary>
        /// Collects actions for a specific agent type.
        /// </summary>
        /// <param name="type">Type of the agent to collect actions for.</param>
        /// <returns>Array of actions for the specified agent type.</returns>
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

        /// <summary>
        /// Collects meta information about actions for a specific agent type.
        /// </summary>
        /// <param name="type">Type of the agent to collect action meta information for.</param>
        /// <returns>Array of action meta information for the specified agent type.</returns>
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

    /// <summary>
    /// Contains meta information about an action, including its method or event, its attribute, and its action info.
    /// </summary>
    public class ActionMetaInfo // (!!) esta clase es para encapsular la info de una accion tanto su methodInfo o eventInfo, su actionAttribute y actionInfo
    {
        /// <summary>Info about the action. </summary>
        public ActionInfo actionInfo;
        /// <summary>Method associated with the action. </summary>
        public MethodInfo methodInfo;
        /// <summary>Event associated with the action.</summary>
        public EventInfo eventInfo;
        /// <summary>Attribute associated with the action.</summary>
        public UtilityActionAttribute atribute;

        /// <summary>
        /// Constructs an action meta info object with the specified action info, method or event, and attribute.
        /// </summary>
        /// <param name="actionInfo">Info about the action.</param>
        /// <param name="methodInfo">Method associated with the action.</param>
        /// <param name="eventInfo">Event associated with the action.</param>
        /// <param name="atribute">Attribute associated with the action.</param>
        public ActionMetaInfo(ActionInfo actionInfo, MethodInfo methodInfo, EventInfo eventInfo, UtilityActionAttribute atribute)
        {
            this.actionInfo = actionInfo;
            this.methodInfo = methodInfo;
            this.eventInfo = eventInfo;
            this.atribute = atribute;
        }
    }

    /// <summary>
    /// Class for observing agents and maintaining a list of active agents.
    /// </summary>
    public class AgentObserver
    {
        /// <summary>Singleton instance of the agent observer. </summary>
        private static AgentObserver instance;
        /// <summary>List of active agents.</summary>
        public List<AgentBeahaviour> Agents = new List<AgentBeahaviour>();

        /// <summary>Gets the singleton instance of the agent observer.</summary>
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

        /// <summary>
        /// Adds an agent to the list of active agents.
        /// </summary>
        /// <param name="agent">Agent to add.</param>
        public void AddAgent(AgentBeahaviour agent)
        {
            Agents.Add(agent);
        }

        /// <summary>
        /// Removes an agent from the list of active agents.
        /// </summary>
        /// <param name="agent">Agent to remove.</param>
        public void RemoveAgent(AgentBeahaviour agent)
        {
            Agents.Remove(agent);
        }
    }
}