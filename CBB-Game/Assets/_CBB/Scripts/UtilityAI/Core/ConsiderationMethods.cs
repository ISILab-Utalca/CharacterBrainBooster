﻿using ArtificialIntelligence.Utility.Actions;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ArtificialIntelligence.Utility
{
    public class ConsiderationMethods
    {
        public struct MethodEvaluation
        {
            public string EvaluatedVariableName;
            public float OutputValue;
        }
        /// <summary>
        /// Provides a way to choose a method to be evaluated on a consideration
        /// </summary>
        /// <returns>all declared public static methods of this class, except for this one</returns>
        public static List<MethodInfo> GetAllMethods()
        {
            var methods = typeof(ConsiderationMethods).GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly).ToList();
            // Remove a method from the list if it is not marked with the attribute
            methods.RemoveAll(m => m.GetCustomAttribute<ConsiderationMethodAttribute>() == null);
            return methods;
        }
        public static List<string> GetAllMethodNames()
        {
               return GetAllMethods().Select(m => m.Name).ToList();
        }
        /// <summary>
        /// Transforms back a method name into a MethodInfo object
        /// </summary>
        /// <param name="methodName">The name of the method being searched</param>
        /// <returns>A <see cref="MethodInfo"/> if the object has </returns>
        public static MethodInfo GetMethodByName(string methodName)
        {
            return GetAllMethods().Find(m => m.Name == methodName);
        }
        
        [ConsiderationMethod("Distance to target")]
        public static MethodEvaluation DistanceToTarget(LocalAgentMemory agentMemory, GameObject target)
        {
            MethodEvaluation methodEvaluation;
            if (target == null)
            {
                methodEvaluation = new MethodEvaluation
                {
                    OutputValue = 0f,
                    EvaluatedVariableName = "No target"
                };
            }
            else
            {
                methodEvaluation = new()
                {
                    EvaluatedVariableName = "Distance to target",
                    OutputValue = Vector3.Distance(agentMemory.transform.position, target.transform.position)
                };
            }

            return methodEvaluation;
        }
        [ConsiderationMethod("Threat heard")]
        public static MethodEvaluation ThreatHeard(LocalAgentMemory agentMemory, GameObject target)
        {
            string targetName = target == null ? "Unknown" : target.name;
            MethodEvaluation methodEvaluation = new()
            {
                EvaluatedVariableName = $"Threat is near ({targetName})",
                OutputValue = agentMemory.HeardObjects.Count > 0 ? 1f : 0f,
            };
            return methodEvaluation;
        }
        [ConsiderationMethod("Idle")]
        public static MethodEvaluation Idle(LocalAgentMemory agentMemory, GameObject target)
        {
            MethodEvaluation methodEvaluation = new()
            {
                EvaluatedVariableName = "Constant",
                OutputValue = 0,
            };
            return methodEvaluation;
        }
    }
    // Create an attribute to be used on the methods
    public class ConsiderationMethodAttribute : System.Attribute
    {
        public string Name { get; private set; }
        public ConsiderationMethodAttribute(string name)
        {
            Name = name;
        }
    }
}

