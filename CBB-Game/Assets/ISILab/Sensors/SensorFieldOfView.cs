using ArtificialIntelligence.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CBB.Lib
{
    public class SensorFieldOfView : SensorBaseClass
    {
        // Individual memory
        public List<GameObject> viewedObjects = new();
        private void OnTriggerEnter(Collider other)
        {
            if (isDebug) Debug.Log($"Object detected: {other.name}");

            HelperFunctions.AddTargetToList(viewedObjects, other.gameObject);
            OnSensorUpdate?.Invoke();
        }
        private void OnTriggerExit(Collider other)
        {
            if (isDebug) Debug.Log($"Object lost: {other.name}");
            HelperFunctions.RemoveTargetFromList(viewedObjects, other.gameObject);
            OnSensorUpdate?.Invoke();
        }
    }
}