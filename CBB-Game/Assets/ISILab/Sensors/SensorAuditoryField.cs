using ArtificialIntelligence.Utility;
using CBB.Lib;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
public class SensorAuditoryField : SensorBaseClass
{
    // Configuration
    [JsonProperty, SerializeField]
    private float fieldRadius = 1f;
    // Individual memory
    [JsonProperty]
    public List<GameObject> heardObjects = new();
    private SensorData sensorData;
    // Private references
    private SphereCollider sphereColl;

    protected override void Awake()
    {
        base.Awake();
        sphereColl = GetComponent<SphereCollider>();

    }
    private void OnTriggerEnter(Collider other)
    {
        if (isDebug) Debug.Log($"Object detected: {other.name}");

        heardObjects.Add(other.gameObject);
        OnSensorUpdate?.Invoke();
    }
    private void OnTriggerExit(Collider other)
    {
        if (isDebug) Debug.Log($"Object lost: {other.name}");
        heardObjects.Remove(other.gameObject);
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
        painter.DrawCilinder(this.transform.position, fieldRadius, 3, Vector3.up, Color.green);
    }
}
