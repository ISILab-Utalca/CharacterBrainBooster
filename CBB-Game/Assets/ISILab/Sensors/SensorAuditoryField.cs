using ArtificialIntelligence.Utility;
using CBB.Lib;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using Utility;

[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
public class SensorAuditoryField : Sensor
{
    // Configuration
    [JsonProperty, SerializeField, SerializeProperty("HearingRadius")]
    private float hearingRadius = 1f;
    // Individual memory
    [JsonProperty]
    public List<GameObject> heardObjects = new();
    private SensorData sensorData;
    // Private references
    [SerializeField]
    private SphereCollider sphereColl;

    public float HearingRadius
    {
        get => hearingRadius;
        set
        {
            hearingRadius = value;
            if (sphereColl != null)
            {
                sphereColl.radius = hearingRadius;
            }
        }
    }
    protected override void Awake()
    {
        base.Awake();
        Debug.Log($"{gameObject.name} {this} hearingRadius to string: " + hearingRadius.ToString());
        Dictionary<string, object> config = new()
        {
            { hearingRadius.ToString(), hearingRadius }
        };
        
        sensorData = new(typeof(SensorAuditoryField),config,null);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (viewLogs) Debug.Log($"Object detected: {other.name}");

        heardObjects.Add(other.gameObject);
        OnSensorUpdate?.Invoke();
    }
    private void OnTriggerExit(Collider other)
    {
        if (viewLogs) Debug.Log($"Object lost: {other.name}");
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
        painter.DrawCilinder(this.transform.position, hearingRadius, 3, Vector3.up, Color.green);
    }

    public override string GetSensorData()
    {
        throw new System.NotImplementedException();
    }
}
