using dnorambu.AI.Utility;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using CBB.Lib;
using Newtonsoft.Json;
using Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ArtificialIntelligence.Utility
{
    /// <summary>
    /// Type of consideration that evaluates accross a range of values 
    /// </summary>
    [CreateAssetMenu(fileName = "New consideration", menuName = "Utility AI/Float consideration")]
    public class UtilityConsideration : ScriptableObject, IDataItem
    {
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
        [Tooltip("The class instance that implements desired methods")]
        [JsonIgnore]
        public UnityEngine.Object ImplementationReference;
        [Tooltip("Methods declared on Implementation Reference")]
        public List<string> m_methods = new();
        // Cache for selected method
        [JsonIgnore]
        public MethodInfo _methodInfo;
        [Tooltip("Set this flags in the inspector to show different methods")]
        [JsonIgnore]
        public BindingFlags flags = BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly;
        [HideInInspector]
        [JsonIgnore]
        public int _selectedMethodIndex;

        [Space]
        [Header("New curve configurations")]
        [HideInInspector]
        public List<Curve> _curveTypes;
        [SerializeReference, SerializeField]
        public Curve _curve;
        [JsonIgnore]
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

        public Curve Curve
        {
            get => _curve;
            set
            {
                _curve = value;
                OnChangeCurve?.Invoke(_curve);
            }
        }

        public event Action<Curve> OnChangeCurve;

        public Evaluation GetValue(LocalAgentMemory agent, GameObject target = null)
        {
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
            if (name == null) Debug.LogWarning("[CONSIDERATION] name is null");
            if (_curve == null) Debug.LogWarning("[CONSIDERATION] _curve is null");
            return new Evaluation(name, value, _curve.Calc(value), methodEvaluation.EvaluatedVariableName, _curve);
        }
        // Display the dropdown and detect changes
        // TODO: Duplicated code (see ConsiderationDrawer -> OnInspectorGUI)
        private void OnValidate()
        {
            ResetMethods();
            UpdateMethodInfo();
        }

        public void ResetCurves(int curveIndex)
        {
            _selectedCurveIndex = curveIndex;
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
        public void UpdateMethodInfo(int methodIndex)
        {
            _selectedMethodIndex = methodIndex;
            UpdateMethodInfo();
        }
        private void ResetMethods()
        {
            m_methods = new();
            var methodInfos = ImplementationReference.GetType().GetMethods(flags);
            foreach (var method in methodInfos)
            {
                m_methods.Add(method.Name);
            }
        }
        private void OnEnable()
        {
#if UNITY_EDITOR
            ResetMethods();
#endif

            UpdateMethodInfo();
            //ResetCurves();
        }

        private void OnDestroy()
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
#endif
        }

        private void OnDisable()
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
#endif
        }

        public void UpdateMethodInfo()
        {
            _methodInfo = ImplementationReference.GetType().GetMethod(m_methods[_selectedMethodIndex]);
        }

#if UNITY_EDITOR
        /// <summary>
        /// Retreive configurations from a json file and update parameters
        /// </summary>
        public void LoadConfiguration()
        {
            // Get the path to the json file
            var path = EditorUtility.OpenFilePanel("Load configuration", Application.dataPath, "json");
            if (path.Length != 0)
            {
                // Read the json file
                var json = System.IO.File.ReadAllText(path);
                // Deserialize the json
                var settings = new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Error,
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                    TypeNameHandling = TypeNameHandling.Auto
                };
                var data = JsonConvert.DeserializeObject<ConsiderationConfiguration>(json, settings);
                // Update the parameters
                SetParamsFromConfiguration(data);
            }
        }

        public void SetParamsFromConfiguration(ConsiderationConfiguration data)
        {
            _curve = data.curve;
            name = data.name;
            m_bookends = data.normalizeInput;
            m_minValue = data.minValue;
            m_maxValue = data.maxValue;
        }

#endif
        public ConsiderationConfiguration GetConfiguration()
        {
            return new ConsiderationConfiguration(name, _curve, m_bookends, m_minValue, m_maxValue);
        }

        public string GetItemName()
        {
            return name;
        }
    }
}
