using ArtificialIntelligence.Utility;
using CBB.Api;
using CBB.DataManagement;
using CBB.Lib;
using Generic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
public class SensorAuditoryField : Sensor, IGeneric
{
    [Header("Memory")]
    [SensorMemory]
    public List<GameObject> heardObjects = new();
    private SensorStatus sensorData;

    [Header("Configurations")]
    [SensorConfiguration, SerializeField, SerializeProperty("HearingRadius")]
    private float hearingRadius = 1f;
    [SensorConfiguration, SerializeField, TagSelector, Tooltip("Which gameObjects with certain tags will this sensor detect")]
    private List<string> hearingTags = new();
    [SensorConfiguration, SerializeField]
    private bool UpdateOnEnter = true;
    [SensorConfiguration, SerializeField]
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
        heardObjects.Add(other.gameObject);
        _agentMemory.HeardObjects.Add(other.gameObject);
        if (!UpdateOnEnter) return;
        if (viewLogs) Debug.Log($"Object detected: {other.name}");


        var name = HelperFunctions.SplitStringUppercase(GetType().Name);
        var sa = new SensorActivation(name, other.gameObject.name, DateTime.Now.ToString(), agentID);
        OnSensorUpdate?.Invoke(sa);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!hearingTags.Contains(other.tag)) return;
        heardObjects.Remove(other.gameObject);
        _agentMemory.HeardObjects.Remove(other.gameObject);
        if (!UpdateOnExit) return;
        if (viewLogs) Debug.Log($"Object exited: {other.name}");


        var name = HelperFunctions.SplitStringUppercase(GetType().Name);
        var sa = new SensorActivation(name, other.gameObject.name, DateTime.Now.ToString(), agentID);
        OnSensorUpdate?.Invoke(sa);
    }

    [ContextMenu("Serialize sensor")]
    public override string SerializeSensor()
    {
        // Create sensor specific data
        var sd = GetSensorData();
        Debug.Log($"Serializing {this}");
        Debug.Log($"Configurations: ");
        foreach (var kvp in sd.configurations)
        {
            Debug.Log($"{kvp.Key} : {kvp.Value}");
        }
        Debug.Log($"Memory: ");
        foreach (var kvp in sd.memory)
        {
            Debug.Log($"{kvp.Key} : {kvp.Value}");
        }
        List<JsonConverter> converters = new()
                {
                    new GameObjectConverter(),
                    new Vector3Converter()
                };
        string ss = JSONDataManager.SerializeData(this, converters);
        Debug.Log(ss);
        return ss;
    }

    protected override void RenderGui(GLPainter painter)
    {
        painter.DrawCircle(this.transform.position, hearingRadius, UnityEngine.Vector3.up, Color.green);
    }

    public override SensorStatus GetSensorData()
    {
        sensorData = new SensorStatus
        {
            sensorType = typeof(SensorAuditoryField),
            configurations = UtilitySystem.CollectSensorConfiguration(this),
            memory = UtilitySystem.CollectSensorMemory(this)
        };
        return sensorData;
    }

    public override void SetParams(DataGeneric data)
    {
        this.HearingRadius = (float)data.FindValueByName("Hearing radius").GetValue();
    }

    public override DataGeneric GetGeneric()
    {
        var data = new DataGeneric(DataGeneric.DataType.Sensor) { ClassType = GetType() };

        data.Add(new WraperNumber { name = "Hearing radius", value = this.HearingRadius });
        return data;
    }
    #endregion
}