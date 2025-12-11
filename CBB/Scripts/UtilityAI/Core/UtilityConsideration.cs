using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using CBB.Lib;
using Newtonsoft.Json;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ArtificialIntelligence.Utility
{
    [Serializable]
    public class UtilityConsideration : IDataItem
    {
        [Tooltip("Name of the consideration")]
        public string considerationName;
        [Header("Bookends")]
        [SerializeField, Tooltip("Set this to true if input for response curve needs to be normalized")]
        private bool m_bookends;
        [Tooltip("Min value for clamping input")]
        [SerializeField]
        private float m_minValue;
        [Tooltip("Max value for clamping input")]
        [SerializeField]
        private float m_maxValue;

        [Header("Method selection")]
        [JsonIgnore]
        public MethodInfo _methodInfo;

        [Space]
        [Header("Curve configurations")]

        [SerializeReference, SerializeField]
        public Curve _curve;

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
            if (_curve == null)
            {
                throw new Exception("[CONSIDERATION] _curve is null. Consideration: ");
            }
            var methodEvaluation = (ConsiderationMethods.MethodEvaluation)_methodInfo.Invoke(null, new object[] { agent, target });
            var value = methodEvaluation.OutputValue;
            if (m_bookends)
            {
                // Clamp input between min and max values defined in bookends
                value = Mathf.Clamp(value, m_minValue, m_maxValue);
                // Normalize it
                value = (value - m_minValue) / (m_maxValue - m_minValue);
            }
            // Return the evaluation
            if (considerationName == null) Debug.LogWarning($"[CONSIDERATION] {this} name is null");
            return new Evaluation(considerationName, value, _curve.Calc(value), methodEvaluation.EvaluatedVariableName, _curve);
        }

        public void UpdateMethodInfo(string methodName)
        {
            _methodInfo = ConsiderationMethods.GetMethodByName(methodName);
        }

        public void SetParamsFromConfiguration(ConsiderationConfiguration data)
        {
            _curve = data.curve;
            considerationName = data.considerationName;
            _methodInfo = ConsiderationMethods.GetMethodByName(data.evaluationMethod);
            m_bookends = data.normalizeInput;
            m_minValue = data.minValue;
            m_maxValue = data.maxValue;
        }

        public ConsiderationConfiguration GetConfiguration()
        {
            return new ConsiderationConfiguration(
                considerationName,
                _curve,
                _methodInfo.Name,
                m_bookends,
                m_minValue,
                m_maxValue
            );
        }

        public string GetItemName() => considerationName;
        public object GetInstance() => this;
    }
}
