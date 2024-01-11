using ArtificialIntelligence.Utility;
using CBB.Lib;
using Generic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace CBB.Lib
{
    [RequireComponent(typeof(BoxCollider))]
    public class SensorFieldOfView : Sensor
    {
        [SerializeField, SerializeProperty("HorizontalFOV")]
        private float horizontalFOV = 1;
        [SerializeField, SerializeProperty("VerticalFOV")]
        private float verticalFOV = 1;
        [SerializeField, SerializeProperty("FrontalFOV")]
        private float frontalFOV = 1;
        // Individual memory
        public List<GameObject> viewedObjects = new();
        // Implementation
        public BoxCollider boxCollider;
        public SensorStatus sensorData;

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
        public float VerticalFOV
        {
            get => verticalFOV;
            set
            {
                verticalFOV = value;
                var boxSize = boxCollider.size;
                if (boxCollider != null)
                {
                    boxCollider.size = new Vector3(boxSize.x, verticalFOV, boxSize.z);
                }
            }
        }
        public float FrontalFOV
        {
            get => frontalFOV;
            set
            {
                frontalFOV = value;
                var boxSize = boxCollider.size;
                var boxCenter = boxCollider.center;
                if (boxCollider != null)
                {
                    boxCollider.size = new Vector3(boxSize.x, boxSize.y, frontalFOV);
                    boxCollider.center = new Vector3(boxCenter.x, boxCenter.y, frontalFOV / 2);
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
            if (viewLogs) Debug.Log($"Object detected: {other.name}");
            viewedObjects.Add(other.gameObject);
            var sa = new SensorActivation(GetType().Name, other.gameObject.name, DateTime.Now.ToString(), agentID);
            OnSensorUpdate?.Invoke(sa);
        }

        private void OnTriggerExit(Collider other)
        {
            if (viewLogs) Debug.Log($"Object lost: {other.name}");
            var sa = new SensorActivation(GetType().Name, other.gameObject.name, DateTime.Now.ToString(), agentID);
            viewedObjects.Remove(other.gameObject);
            OnSensorUpdate?.Invoke(sa);
        }

        [ContextMenu("Serialize sensor")]
        public override string SerializeSensor()
        {
            string ss = JSONDataManager.SerializeData(this);
            Debug.Log(ss);
            return ss;
        }

        protected override void RenderGui(GLPainter painter)
        {
            painter.DrawCilinder(this.transform.position, 5, 3, Vector3.up, Color.red);
        }

        public override SensorStatus GetSensorData()
        {
            throw new System.NotImplementedException();
        }

        public override void SetParams(DataGeneric data)
        {
            this.HorizontalFOV = (float)data.FindValueByName("HorizontalFOV").Getvalue();
            this.VerticalFOV = (float)data.FindValueByName("VerticalFOV").Getvalue();
            this.FrontalFOV = (float)data.FindValueByName("FrontalFOV").Getvalue();
        }

        public override DataGeneric GetGeneric()
        {
            var data = new DataGeneric(DataGeneric.DataType.Sensor) { ClassType = typeof(SensorFieldOfView) };
            data.Add(new WraperNumber { name = "HorizontalFOV", value = HorizontalFOV });
            data.Add(new WraperNumber { name = "VerticalFOV", value = VerticalFOV });
            data.Add(new WraperNumber { name = "FrontalFOV", value = FrontalFOV });
            return data;
        }
    }

    public class SensorFieldOfViewConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(SensorFieldOfView);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var value = serializer.Deserialize<SensorFieldOfView>(reader);
            return value;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var sensor = (SensorFieldOfView)value;

            writer.WriteStartObject();
            writer.WritePropertyName("HorizontalFOV");
            writer.WriteValue(sensor.HorizontalFOV);
            writer.WritePropertyName("VerticalFOV");
            writer.WriteValue(sensor.VerticalFOV);
            writer.WritePropertyName("FrontalFOV");
            writer.WriteValue(sensor.FrontalFOV);
            writer.WriteEndObject();
        }
    }
}