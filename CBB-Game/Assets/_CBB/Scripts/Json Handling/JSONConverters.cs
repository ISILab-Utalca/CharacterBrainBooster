using CBB.Lib;
using Newtonsoft.Json;
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
        writer.WritePropertyName("Target Name");
        writer.WriteValue(targetGameObject.name);
        Vector3 targetPosition = targetGameObject.transform.position;
        writer.WritePropertyName("Target Position X");
        writer.WriteValue(targetPosition.x);
        writer.WritePropertyName("Target Position Y");
        writer.WriteValue(targetPosition.y);
        writer.WritePropertyName("Target Position Z");
        writer.WriteValue(targetPosition.z);
        writer.WriteEndObject();
    }
}