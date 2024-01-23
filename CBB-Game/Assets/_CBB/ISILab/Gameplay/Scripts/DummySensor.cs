using ArtificialIntelligence.Utility;
using CBB.Api;
using CBB.Lib;
using Generic;
using System;
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



    public override SensorStatus GetSensorData()
    {
        return new SensorStatus
        {
            sensorType = typeof(DummySensor),
            configurations = UtilitySystem.CollectSensorConfiguration(this),
            memory = UtilitySystem.CollectSensorMemory(this)
        };
    }

    public override string SerializeSensor()
    {
        throw new NotImplementedException();
    }

    public override void SetParams(DataGeneric data)
    {
        this.isAwake = (bool)data.FindValueByName("isAwake").Getvalue();
    }

    public override DataGeneric GetGeneric()
    {
        var data = new DataGeneric(DataGeneric.DataType.Sensor) { ClassType = GetType() };

        data.Add(new WraperBoolean { name = "isAwake", value = this.isAwake });
        return data;
    }

    protected override void RenderGui(GLPainter painter)
    {
        throw new NotImplementedException();
    }
}
