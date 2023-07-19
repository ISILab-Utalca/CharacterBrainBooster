using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;

namespace ArtificialIntelligence.Utility
{
    public static class HelperFunctions
    {

        public static Texture GetTexture(string nameOfTexture)
        {
            var textures = Resources.FindObjectsOfTypeAll(typeof(Texture))
                .Where(t => t.name.ToLower().Contains(nameOfTexture))
                .Cast<Texture>().ToList();

            if (textures.Count == 0)
            {
                textures = Resources.FindObjectsOfTypeAll(typeof(Texture))
                .Where(t => t.name.ToLower().Contains("heart"))
                .Cast<Texture>().ToList();

            }
            return textures[0];


        }
        /// <summary>
        /// Get the first component of type T in the hierarchy of the given GameObject:
        /// <list type="bullet">
        /// <item>gameObject itself </item>
        /// <item>Parents of gameObject </item>
        /// <item>Children of gameObject </item>
        /// </list>
        /// </summary>
        /// <returns>The Brain component of the agent.</returns>
        public static T GetComponentOnHierarchy<T>(this GameObject gameObj) where T : Component
        {
            T component = null;
            if (gameObj.TryGetComponent(out T componentOnGameObject))
            {
                component = componentOnGameObject;
            }
            else
            {
                component = gameObj.GetComponentInParent<T>();
                if (component == null)
                {
                    component = gameObj.GetComponentInChildren<T>();
                }
            }
            return component;
        }
        /// <summary>
        /// Retrieves all the components of type T on the gameObject, its parents and its children.
        /// Removes duplicates in case of any.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="gameObj"></param>
        /// <returns></returns>
        public static List<T> GetComponentsOnHierarchy<T>(this GameObject gameObj) where T : Component
        {
            List<T> components = new();
            List<T> tempComp = gameObj.GetComponentsInParent<T>().ToList();
            components.AddRange(tempComp);
            tempComp = gameObj.GetComponentsInChildren<T>().ToList();
            components.AddRange(tempComp);
            components = components.Distinct().ToList();
            return components;
        }
        public static bool CheckTargetInList<T>(List<T> targets, T target)
        {
            return targets != null ? targets.Contains(target) : false;
        }
        public static void AddTargetToList<T>(List<T> targets, T target)
        {
            targets.Add(target);
        }
        public static void RemoveTargetFromList<T>(List<T> targets, T target)
        {
            targets.Remove(target);
        }
        /// <summary>
        /// Extension method to evaluate if a NavmeshAgent arrived to its destination
        /// </summary>
        /// <param name="agent"></param>
        /// <returns><b>true</b> when agent is on destination, else <b>false</b></returns>
        public static bool ReachedDestination(this NavMeshAgent agent)
        {
            if (!agent.pathPending)
            {
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }



}
