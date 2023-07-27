using System;
using UnityEngine;
using CBB.InternalTool;

namespace ArtificialIntelligence.Utility
{
    /// <summary>
    /// Base class for every sensor that an Agent can have.
    /// A "Sensor" should only care (update) about one aspect (property)
    /// of the Agent attached to, e.g. it's health.
    /// </summary>
    public abstract class SensorBaseClass : MonoBehaviour, ISensor
    {
        // Event for when the sensor detects something
        public System.Action OnSensorUpdate;
        protected LocalAgentMemory _agentMemory;
        protected bool isDebug = false;

        // GUI
        private static GLPainter painter = new GLPainter();

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

        public bool CheckForParentBrain()
        {
            throw new System.NotImplementedException();
        }

        public void CheckForValue()
        {
            throw new System.NotImplementedException();
        }

        private void InternalGUI(object obj)
        {
            if (Settings.ShowGUI) 
                RenderGui(painter);
        }

        protected abstract void RenderGui(GLPainter painter);

        public abstract string GetSensorData();
    }
}

