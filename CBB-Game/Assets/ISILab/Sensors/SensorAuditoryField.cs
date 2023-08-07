using ArtificialIntelligence.Utility;
using CBB.Api;
using CBB.Lib;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using Utility;

[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
public class SensorAuditoryField : Sensor
{
    [Header("Memory")]
    [SensorMemory]
    public List<GameObject> heardObjects = new();
    private SensorData sensorData;

    [Header("Configurations")]
    [SensorConfiguration, SerializeField, SerializeProperty("HearingRadius")]
    private float hearingRadius = 1f;
    [SensorConfiguration, SerializeField, TagSelector, Tooltip("Which gameObjects with certain tags will this sensor detect")]
    private List<string> hearingTags = new();
    [SensorConfiguration,SerializeField]
    private bool UpdateOnEnter = true;
    [SensorConfiguration,SerializeField]
    private bool UpdateOnExit = false;
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

    #region Methods
    protected override void Awake()
    {
        base.Awake();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!hearingTags.Contains(other.tag)) return;
        if (viewLogs) Debug.Log($"Object detected: {other.name}");
        heardObjects.Add(other.gameObject);
        _agentMemory.HeardObjects.Add(other.gameObject);
        if(UpdateOnEnter) OnSensorUpdate?.Invoke();
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (!hearingTags.Contains(other.tag)) return;
        if (viewLogs) Debug.Log($"Object lost: {other.name}");
        heardObjects.Remove(other.gameObject);
        _agentMemory.HeardObjects.Remove(other.gameObject);
        if (UpdateOnExit) OnSensorUpdate?.Invoke();
    }
    
    [ContextMenu("Serialize sensor")]
    public void SerializeSensor()
    {
        string ss = JSONDataManager.SerializeData(this);
        Debug.Log(ss);
    }

    protected override void RenderGui(GLPainter painter)
    {
        painter.DrawCircle(this.transform.position, hearingRadius, Vector3.up, Color.green);
    }

    public override SensorData GetSensorData()
    {
        sensorData = new SensorData
        {
            sensorType = typeof(SensorAuditoryField),
            configurations = UtilitySystem.CollectSensorConfiguration(this),
            memory = UtilitySystem.CollectSensorMemory(this)
        };
        return sensorData;
    }
    #endregion
}
