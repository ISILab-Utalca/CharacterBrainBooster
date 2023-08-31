using ArtificialIntelligence.Utility;
using CBB.Api;
using CBB.Lib;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummySensor : Sensor
{
    // Some random variables for its configuration
    //[SensorConfiguration]
    //private float floatVar = 0f;
    //[SensorConfiguration]
    //private int intVar = 0;
    [SensorConfiguration]
    public List<GameObject> randomObjects;
    [SensorConfiguration]
    public bool isAwake = true;
    [SensorConfiguration]
    public string StringProperty {  get; set; }


    public override SensorData GetSensorData()
    {
        StringProperty = "Hola";
        return new SensorData
        {
            sensorType = typeof(DummySensor),
            configurations = UtilitySystem.CollectSensorConfiguration(this),
            memory = UtilitySystem.CollectSensorMemory(this)
        };
    }

    protected override void RenderGui(GLPainter painter)
    {
        throw new NotImplementedException();
    }
}
