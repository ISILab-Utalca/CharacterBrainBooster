using ArtificialIntelligence.Utility;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace CBB.Lib
{
    [RequireComponent(typeof(BoxCollider))]
    public class SensorFieldOfView : SensorBaseClass
    {
        [SerializeField, SerializeProperty("HorizontalFOV")]
        private float horizontalFOV = 1;
        private float verticalFOV = 1;
        private float rangeOfSight = 1;
        // Individual memory
        public List<GameObject> viewedObjects = new();
        // Implementation
        public BoxCollider boxCollider;
        public SensorData sensorData;

        public float HorizontalFOV
        {
            get => horizontalFOV;
            set
            {
                horizontalFOV = value;
                var boxSize = boxCollider.size;
                if (boxCollider != null)
                {
                    boxCollider.size = new Vector3(HorizontalFOV, boxSize.y, boxSize.z);
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();
            boxCollider = GetComponent<BoxCollider>();

        }

        private void OnTriggerEnter(Collider other)
        {
            if (isDebug) Debug.Log($"Object detected: {other.name}");

            viewedObjects.Add(other.gameObject);
            OnSensorUpdate?.Invoke();
        }

        private void OnTriggerExit(Collider other)
        {
            if (isDebug) Debug.Log($"Object lost: {other.name}");
            viewedObjects.Remove(other.gameObject);
            OnSensorUpdate?.Invoke();
        }

        [ContextMenu("Serialize sensor")]
        public void SerializeSensor()
        {
            string ss = JSONDataManager.SerializeData(this);
            Debug.Log(ss);
        }

        protected override void RenderGui(GLPainter painter)
        {
            painter.DrawCilinder(this.transform.position, 5, 3, Vector3.up, Color.red);
        }
        public override string GetSensorData()
        {
            throw new System.NotImplementedException();
        }
    }
}