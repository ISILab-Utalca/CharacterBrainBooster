using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CBB.Api
{

    // (??) esta clase se podria inyectar al momento de ser instanciado el objeto que lo necesite
    // asi evitamos que los usuarios tengan que acordarse de poner ellos esta clase a mano
    public class AgentBehaviour : MonoBehaviour // brain
    {
        private float lastTime = 0;
        private float cooldown = 50; // (?) ms o seg ?

        private List<Utility> utilities = new();

        private void Awake()
        {
            AgentObserver.Instance.AddAgent(this);
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

        public bool IsAvailable()
        {
            if ((Time.time - lastTime) > cooldown)
            {
                lastTime = Time.time;
                return true;
            }
            return false;
        }

        private List<Variable> GetInputs(object behaviour, Type ownerType)
        {
            var inputs = new List<Variable>();

            var fields = behaviour.GetType().GetFields();
            foreach (var field in fields)
            {
                var atts = field.GetCustomAttributes();
                if (atts.Any(a => a.GetType() == typeof(UtilityInputAttribute)))
                {
                    var variable = new Variable(field.Name, field.FieldType, ownerType);
                    inputs.Add(variable);
                }
            }

            var props = behaviour.GetType().GetProperties();
            foreach (var prop in props)
            {
                var atts = prop.GetCustomAttributes();
                if (atts.Any(a => a.GetType() == typeof(UtilityInputAttribute)))
                {
                    var variable = new Variable(prop.Name, prop.PropertyType, ownerType);
                    inputs.Add(variable);
                }
            }

            return inputs;
        }

        private List<ActionInfo> GetActions(object behaviour, Type ownerType)
        {
            var actions = new List<ActionInfo>();
            var meths = behaviour.GetType().GetMethods();
            foreach (var meth in meths)
            {
                var atts = meth.GetCustomAttributes();
                if (atts.Any(a => a.GetType() == typeof(UtilityActionAttribute)))
                {
                    var action = new ActionInfo(meth.Name, ownerType);
                    actions.Add(action);
                }
            }

            var events = behaviour.GetType().GetEvents();
            foreach (var evt in events)
            {
                var atts = evt.GetCustomAttributes();
                if (atts.Any(a => a.GetType() == typeof(UtilityActionAttribute)))
                {
                    var action = new ActionInfo(evt.Name, ownerType);
                    actions.Add(action);
                }
            }

            return actions;
        }

        /// <summary>
        /// Searches for and returns the first component of type 'MonoBehaviour' that has the 'UtilityAgentAttribute'
        /// attribute on a given object. If no such component is found, 'null' is returned.
        /// </summary>
        /// <returns>The 'MonoBehaviour' component with the 'UtilityAgentAttribute' attribute, or 'null' if not found.</returns>
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