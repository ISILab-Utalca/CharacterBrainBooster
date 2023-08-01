using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

namespace ArtificialIntelligence.Utility
{
    /// <summary>
    /// Type of consideration that evaluates accross a range of values 
    /// </summary>
    [CreateAssetMenu(fileName = "New consideration", menuName = "Utility AI/Float consideration")]
    public class UtilityConsideration : ScriptableObject
    {
        [Header("Bookends")]
        [SerializeField, Tooltip("Set this to true if input for response curve needs to be normalized")]
        private bool _bookends;
        [Tooltip("Min value for clamping input")]
        [SerializeField] private float _minValue;
        [Tooltip("Max value for clamping input")]
        [SerializeField] private float _maxValue;
        [Space]
        [SerializeField] private AnimationCurve _responseCurve;
        [Space]
        [Header("Method selection")]
        [Tooltip("The class instance that implements desired methods")]
        public Object ImplementationReference;
        [Tooltip("Methods declared on Implementation Reference")]
        public List<string> _methods = new();
        // Cache for selected method
        public MethodInfo _methodInfo;
        [Tooltip("Set this flags in the inspector to show different methods")]
        public BindingFlags flags = BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly;
        [HideInInspector]
        public int _selectedMethodIndex;
        public float GetValue(LocalAgentMemory agent, GameObject target = null)
        {
            var value = (float)_methodInfo.Invoke(null, new object[] { agent, target });
            if (_bookends)
            {
                // Clamp input between min and max values defined in bookends
                value = Mathf.Clamp(value, _minValue, _maxValue);
                // Normalize it
                value = (value - _minValue) / (_maxValue - _minValue);
            }
            // Return the evaluation
            return _responseCurve.Evaluate(value);
        }
        private void OnValidate()
        {
            ResetMethods();
        }

        private void ResetMethods()
        {
            _methods = new();
            var methodInfos = ImplementationReference.GetType().GetMethods(flags);
            foreach (var method in methodInfos)
            {
                _methods.Add(method.Name);
            }
        }
        private void OnEnable()
        {
#if UNITY_EDITOR
            ResetMethods();
#endif

            UpdateMethodInfo();
        }

        public void UpdateMethodInfo()
        {
            _methodInfo = ImplementationReference.GetType().GetMethod(_methods[_selectedMethodIndex]);
        }
    }
}

