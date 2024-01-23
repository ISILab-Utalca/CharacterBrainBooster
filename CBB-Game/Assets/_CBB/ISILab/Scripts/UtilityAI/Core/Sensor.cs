using CBB.InternalTool;
using CBB.InternalTool.DebugTools;
using CBB.Lib;
using Generic;
using System;
using UnityEngine;

namespace ArtificialIntelligence.Utility
{
    /// <summary>
    /// Base class for every sensor that an Agent can have.
    /// A "Sensor" should only care (update) about one aspect (property)
    /// of the Agent attached to, e.g. it's health.
    /// </summary>
    public abstract class Sensor : MonoBehaviour, ISensor, IGeneric
    {
        // Event for when the sensor detects something
        protected LocalAgentMemory _agentMemory;
        public SensorStatus SensorData = new();
        [SerializeField]
        protected bool viewLogs = false;
        protected int agentID = 1;
        // GUI
        private static GLPainter painter = new GLPainter();

        public Action<SensorActivation> OnSensorUpdate { get; set; }

        protected virtual void Awake()
        {
            _agentMemory = gameObject.GetComponentOnHierarchy<LocalAgentMemory>();
            agentID = gameObject.GetInstanceID();
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
            if(Settings.ShowGUI) RenderGui(painter);
        }

        protected abstract void RenderGui(GLPainter painter);

        public abstract SensorStatus GetSensorData();

        public abstract string SerializeSensor();

        public abstract void SetParams(DataGeneric data);

        public abstract DataGeneric GetGeneric();
    }
}

