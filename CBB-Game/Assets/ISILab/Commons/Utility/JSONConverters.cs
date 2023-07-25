using CBB.Lib;
using Newtonsoft.Json;
using System;
using UnityEngine;

public class SensorFieldOfViewConverter : JsonConverter
{
    public override bool CanConvert(System.Type objectType)
    {
        return objectType == typeof(SensorFieldOfView);
    }

    public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer)
    {
        var value = serializer.Deserialize(reader);
        return JsonConvert.DeserializeObject<SensorFieldOfView>(value.ToString());
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var sensor_fov = (SensorFieldOfView)value;

        writer.WriteStartObject();
        writer.WritePropertyName("ownerAgent");
        writer.WriteValue(sensor_fov.gameObject.name);
        writer.WritePropertyName("viewedObjects");
        writer.WriteValue(sensor_fov.viewedObjects);
        writer.WriteEndObject();
    }
}
public class TransformClassConverter : JsonConverter
{
    public override bool CanConvert(System.Type objectType)
    {
        return objectType == typeof(Transformer);
    }

    public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer)
    {
        var value = serializer.Deserialize(reader);
        return JsonConvert.DeserializeObject<Transformer>(value.ToString());
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var transformer = (Transformer)value;

        writer.WriteStartObject();
        writer.WritePropertyName("test");
        writer.WriteValue(transformer.tests);
        writer.WriteEndObject();
    }
}

public class GameObjectConverter : JsonConverter
{
    public override bool CanConvert(System.Type objectType)
    {
        return objectType == typeof(GameObject);
    }

    public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer)
    {
        var value = serializer.Deserialize(reader);
        return JsonConvert.DeserializeObject<GameObject>(value.ToString());
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var targetGameObject = (GameObject)value;

        writer.WriteStartObject();
        writer.WritePropertyName("targetName");
        writer.WriteValue(targetGameObject.name);
        writer.WritePropertyName("targetPosition");
        writer.WriteValue(targetGameObject.transform.position);
        writer.WriteEndObject();
    }
}