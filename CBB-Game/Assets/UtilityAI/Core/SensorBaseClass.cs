using ArtificialIntelligence.Utility;
using MixTheForgotten.Architecture;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MixTheForgotten.AI.Sensors
{
    /// <summary>
    /// Base class for every sensor that an Agent can have.
    /// A "Sensor" should only care (update) about one aspect (property)
    /// of the Agent attached to, e.g. it's health.
    /// </summary>
    public abstract class SensorBaseClass : MonoBehaviour, ISensible
    {
        // Event for when the sensor detects something
        public System.Action OnSensorUpdate;
        protected LocalAgentMemory _agentMemory;

        protected virtual void Awake()
        {
            _agentMemory = gameObject.GetComponentOnHierarchy<LocalAgentMemory>();
            
        }
        public bool CheckForParentBrain()
        {
            throw new System.NotImplementedException();
        }

        public void CheckForValue()
        {
            throw new System.NotImplementedException();
        }
    }
}

