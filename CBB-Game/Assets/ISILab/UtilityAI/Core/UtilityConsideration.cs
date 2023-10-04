using dnorambu.AI.Utility;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

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
        [SerializeField]
        private float _minValue;
        [Tooltip("Max value for clamping input")]
        [SerializeField]
        private float _maxValue;

        [Header("Method selection")]
        [Tooltip("The class instance that implements desired methods")]
        public UnityEngine.Object ImplementationReference;
        [Tooltip("Methods declared on Implementation Reference")]
        public List<string> _methods = new();
        // Cache for selected method
        public MethodInfo _methodInfo;
        [Tooltip("Set this flags in the inspector to show different methods")]
        public BindingFlags flags = BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly;
        [HideInInspector]
        public int _selectedMethodIndex;

        [Space]
        [Header("New curve configurations")]
        [HideInInspector]
        public List<Curve> _curveTypes;
        public Curve _curve;
        public int _selectedCurveIndex;
        public struct Evaluation
        {
            public string ConsiderationName;
            public float InputValue;
            public float UtilityValue;
            public string InputName; // Variable being evaluated on the consideration
            [SerializeReference]
            public Curve Curve;
            public Evaluation(string consName, float inputValue, float utilityValue, string inputName, Curve curve)
            {
                ConsiderationName = consName;
                InputValue = inputValue;
                UtilityValue = utilityValue;
                InputName = inputName;
                Curve = curve;
            }
        }
        public Evaluation GetValue(LocalAgentMemory agent, GameObject target = null)
        {
            var methodEvaluation = (ConsiderationMethods.MethodEvaluation)_methodInfo.Invoke(null, new object[] { agent, target });
            var value = methodEvaluation.OutputValue;
            if (_bookends)
            {
                // Clamp input between min and max values defined in bookends
                value = Mathf.Clamp(value, _minValue, _maxValue);
                // Normalize it
                value = (value - _minValue) / (_maxValue - _minValue);
            }
            // Return the evaluation
            if (name == null) Debug.LogWarning("[CONSIDERATION] name is null");
            if (_curve == null) Debug.LogWarning("[CONSIDERATION] _curve is null");
            return new Evaluation(name,value, _curve.Calc(value), methodEvaluation.EvaluatedVariableName, _curve);
        }
        // Display the dropdown and detect changes
        // TODO: Duplicated code (see ConsiderationDrawer -> OnInspectorGUI)
        private void OnValidate()
        {
            ResetMethods();
            UpdateMethodInfo();

            // Curves logic
            ResetCurves();
            
        }


        public void ResetCurves()
        {
            // Get all the curves classes names and add them to the list
            _curveTypes = Curve.GetCurves();
            var curveType = _curveTypes[_selectedCurveIndex].GetType();
            var curveInstance = Activator.CreateInstance(curveType);
            _curve = curveInstance switch
            {
                Linear l => l,
                ExponencialInvertida ei => ei,
                Exponencial exp => exp,
                Staggered stg => stg,
                Sigmoide sigmoide => sigmoide,
                Constant constant => constant,
                Bell bell => bell,
                _ => null,
            };
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
            ResetCurves();
        }

        public void UpdateMethodInfo()
        {
            _methodInfo = ImplementationReference.GetType().GetMethod(_methods[_selectedMethodIndex]);
        }
    }
}

