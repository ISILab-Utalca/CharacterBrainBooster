using CBB.InternalTool;
using CBB.Lib;
using System;
using UnityEngine;

namespace ArtificialIntelligence.Utility
{
    /// <summary>
    /// Base class for every sensor that an Agent can have.
    /// A "Sensor" should only care (update) about one aspect (property)
    /// of the Agent attached to, e.g. it's health.
    /// </summary>
    public abstract class Sensor : MonoBehaviour, ISensor
    {
        // Event for when the sensor detects something
        protected LocalAgentMemory _agentMemory;
        public SensorData SensorData = new();
        [SerializeField]
        protected bool viewLogs = false;
        
        // GUI
        private static GLPainter painter = new GLPainter();

        public Action OnSensorUpdate { get ; set; }

        protected virtual void Awake()
        {
            _agentMemory = gameObject.GetComponentOnHierarchy<LocalAgentMemory>();
        }

        protected void OnEnable()
        {
            Camera.onPostRender += InternalGUI;
        }

        protected void OnDisable()
        {
            Camera.onPostRender -= InternalGUI;
            Camera.onPostRender = null;
        }
        private void InternalGUI(object obj)
        {
            if (Settings.ShowGUI) 
                RenderGui(painter);
        }

        protected abstract void RenderGui(GLPainter painter);

        public abstract SensorData GetSensorData();
    }
}

