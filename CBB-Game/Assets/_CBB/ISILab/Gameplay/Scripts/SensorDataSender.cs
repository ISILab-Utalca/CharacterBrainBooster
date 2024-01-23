using ArtificialIntelligence.Utility;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class SensorDataSender : MonoBehaviour
{

    [ContextMenu("Log Sensor Data")]
    public void LogSensorData()
    {
        var sensor = gameObject.GetComponent<Sensor>();
        var sensorData = sensor.GetSensorData();

        Debug.Log("Sensor Data:");
        Debug.Log($"Serializing {this}");
        Debug.Log($"Configurations: ");
        foreach (var kvp in sensorData.configurations)
        {
            Debug.Log($"{kvp.Key} : {kvp.Value}");
        }
        Debug.Log($"Memory: ");
        foreach (var kvp in sensorData.memory)
        {
            Debug.Log($"{kvp.Key} : {kvp.Value}");
        }
    }
    [ContextMenu("Serialize sensor data")]
    public void SerializeSensor()
    {
        var sensor = gameObject.GetComponent<Sensor>();
        var sensorData = sensor.GetSensorData();
        List<JsonConverter> converters = new()
                {
                    new GameObjectConverter(),
                    new Vector3Converter()
                };
        string ss = JSONDataManager.SerializeData(sensorData, converters);
        Debug.Log(ss);
    }
    [ContextMenu("Serialize vector list")]
    public void SerializeVectorList()
    {
        List<Vector3> vector3List = new() { Vector3.zero, Vector3.one, Vector3.up };
        Debug.Log(JSONDataManager.SerializeData(vector3List, new Vector3Converter()));
    }
    [ContextMenu("Serialize gameObject list")]
    public void SerializeGameObjectList()
    {
        List<GameObject> gameObjectList = new() {
            Instantiate(GameObject.CreatePrimitive(PrimitiveType.Sphere)),
            Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube)),
        };
        List<JsonConverter> converters = new()
                {
                    new GameObjectConverter(),
                    new Vector3Converter()
                };
        Debug.Log(JSONDataManager.SerializeData(gameObjectList, converters));
    }
}
