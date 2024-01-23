using Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.AI;
#if UNITY_EDITOR
using UnityEngine.UIElements;
#endif
namespace ArtificialIntelligence.Utility
{
    public static class HelperFunctions
    {

        /// <summary>
        /// Print an array of elements in one line, sourrounded by brackets
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static string PrintArray<T>(IEnumerable<T> array)
        {
            string result = "[";
            foreach (var item in array)
            {
                result += item + ", ";
            }
            result += "]";
            return result;
        }
        /// <summary>
        /// Split a string by uppercase letters
        /// </summary>
        /// <param name="s">The string like "MyClassName"</param>
        /// <returns></returns>
        public static string SplitStringUppercase(string s)
        {
            return string.Concat(s.Select(x => char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
        }
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
        /// <returns>The Brain component of the agent. null if not found</returns>
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
        public static bool TargetIsInRange(Transform agent, Transform target, float range)
        {
            return Vector3.Distance(target.position, agent.position) < range;
        }
#if UNITY_EDITOR
        // Menu item function to log the Application Data Path
        [UnityEditor.MenuItem("Tools/Log Application Data Path")]
        public static void LogApplicationDataPath()
        {
            Debug.Log($"Application dataPath:\n{Application.dataPath}");
        }
        // Menu item function to log the Persistent Data Path
        [UnityEditor.MenuItem("Tools/Log Persistent Data Path")]
        public static void LogPersistentDataPath()
        {
            Debug.Log($"Application persistentDataPath:\n{Application.persistentDataPath}");
        }
        // Menu item function find all the subclasses of ActionState
        [UnityEditor.MenuItem("Tools/Log action types")]
        public static void GetAllActions()
        {
            GetInheritedClasses<ActionState>(true);
        }
        [UnityEditor.MenuItem("Tools/Debug data loader path")]
        public static void GetAllSensors()
        {
            Debug.Log("Data Loader PATH: " + DataLoader.Path);
        }
        [UnityEditor.MenuItem("Tools/Log consideration methods")]
        public static void LogAllConsiderationMethods()
        {
            Debug.Log("All consideration methods: ");
            foreach (var item in ConsiderationMethods.GetAllMethodNames())
            {
                Debug.Log(item);
            }
        }
#endif
        /// <summary>
        /// Get all the classes that inherit from T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>A List<typeparamref name="T"/> with all non-abstract derived types</returns>
        public static List<System.Type> GetInheritedClasses<T>(bool showLogs = false) where T : class
        {
            var actionClasses = new List<System.Type>();
            var types = System.AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes()).Where(x => x.IsSubclassOf(typeof(T)) && !x.IsAbstract);

            foreach (var type in types)
            {
                if (showLogs) Debug.Log(type.Name);
                actionClasses.Add(type);
            }

            return actionClasses;

        }
        /// <summary>
        /// Get all classes that implement the interface T and inherit from baseClass
        /// </summary>
        public static List<System.Type> GetInterfaceImplementations<T>(System.Type baseClass, bool showLogs = false)
        {
            var actionClasses = new List<System.Type>();
            var types = System.AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes()).Where(x => x.IsSubclassOf(baseClass) && !x.IsAbstract);

            foreach (var type in types)
            {
                if (showLogs) Debug.Log(type.Name);
                actionClasses.Add(type);
            }

            return actionClasses;

        }
        /// <summary>
        /// Remove the namespace from the class name and split it by capital letters
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public static string RemoveNamespaceSplit(string className)
        {
            string pointPattern = @"[^.]*$";
            Match match = Regex.Match(className, pointPattern);
            if (match.Success)
            {
                // Split by capital letters. Ej: "MyBrain" -> "My", "Brain"
                string capitalPattern = @"(?=\p{Lu})";
                string[] result = Regex.Split(match.Value, capitalPattern);
                // Join the words in the array with a space
                return string.Join(" ", result);
            }
            return className + "{An error ocurred}";
        }

        public static void LoadFromGeneric<T>(List<DataGeneric> container) where T : class
        {
            container.Clear();
            // Find all derived from T
            var dataGenericImplementators = GetInheritedClasses<T>();
            var dummygameObject = new GameObject();
            // Get all the data generic from the actions
            foreach (var item in dataGenericImplementators)
            {
                // In this case, al items are monobehaviours
                var instance = dummygameObject.AddComponent(item);
                // Get the data generic from the action
                var data = ((IGeneric)instance).GetGeneric();
                // Add the data generic to the brain editor
                container.Add(data);
            }
            UnityEngine.Object.DestroyImmediate(dummygameObject);
        }
    }
}
