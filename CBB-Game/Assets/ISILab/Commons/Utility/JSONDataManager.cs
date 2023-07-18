using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

namespace Utility
{
    public static class JSONDataManager
    {
        private static void SaveData<T>(string path, T data)
        {
            // generate json string
            var jsonString = SerializeData(data);

            // write json in a file
            using StreamWriter writer = new StreamWriter(path);
            writer.Write(jsonString);
        }
        public static string SerializeData<T>(T data)
        {
            // generate serializer setting
            var jsonSerializerSettings = new JsonSerializerSettings()
            {
                PreserveReferencesHandling = PreserveReferencesHandling.All,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                TypeNameHandling = TypeNameHandling.All,
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
            };

            // add converters to serializer
            jsonSerializerSettings.Converters.Add(new Vector3Converter());
            jsonSerializerSettings.Converters.Add(new Vector2Converter());

            // generate json string
            return JsonConvert.SerializeObject(
                data,
                jsonSerializerSettings
                );
        }
        public static void SaveData<T>(string directoryName, string fileName, T data)
        {
            string directoryPath = directoryName;
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            string dataPath = directoryPath + '/' + fileName;
            if (File.Exists(dataPath))
            {
                File.Delete(dataPath);
            }

            SaveData(dataPath, data);
        }

        public static void SaveData<T>(string directoryName, string fileName, string format, T data)
        {
            string directoryPath = directoryName;
            if(!Directory.Exists(directoryPath)) 
            {
                Directory.CreateDirectory(directoryPath);
            }
            string dataPath =  directoryPath + '/' + fileName + "." + format;
            if (File.Exists(dataPath))
            {
                File.Delete(dataPath);
            }

            SaveData(dataPath,data);
        }

        private static T LoadData<T>(string path)
        {
            // read file and obtain json string
            using StreamReader reader = new StreamReader(path);
            string json = reader.ReadToEnd();

            // generate serializer setting
            var jsonSerializerSettings = new JsonSerializerSettings()
            {
                PreserveReferencesHandling = PreserveReferencesHandling.All,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                TypeNameHandling = TypeNameHandling.All,
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
            };

            // add converters to serializer
            jsonSerializerSettings.Converters.Add(new Vector3Converter());
            jsonSerializerSettings.Converters.Add(new Vector2Converter());

            // generate data from string
            var data = JsonConvert.DeserializeObject<T>(
                json,
                jsonSerializerSettings
                );

            if (data == null)
                Debug.LogWarning("Data in " + path + " is not of type " + typeof(T).ToString());

            return data;
        }

        public static T LoadData<T>(string directoryName, string fileName)
        {
            string directoryPath = directoryName;
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            string dataPath = directoryPath + '/' + fileName;

            return LoadData<T>(dataPath);
        }

        public static T LoadData<T>(string directoryName, string fileName, string format)
        {
            string directoryPath = directoryName;
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            string dataPath = directoryPath + '/' + fileName +"."+ format;

            return LoadData<T>(dataPath);
        }

        public static List<string> GetJSONFiles(string path)
        {
            if(!Directory.Exists(path))
            {
                return null;
                //return new List<string>(); // (??) return empty list
            }

            string[] files = System.IO.Directory.GetFiles(path);
            List<string> jsonFiles = new List<string>();
            foreach (string s in files)
            {
                if (s.EndsWith(".json"))
                {
                    string[] lines = s.Split('/');
                    jsonFiles.Add(lines[^1].Split('\\')[^1]);
                }
            }

            return jsonFiles;
        }


    }

    public class Vector3Converter : JsonConverter
    {
        public override bool CanConvert(System.Type objectType)
        {
            return objectType == typeof(Vector3);
        }

        public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer)
        {
            var value = serializer.Deserialize(reader);
            return JsonConvert.DeserializeObject<Vector3>(value.ToString());
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var vector3 = (Vector3)value;

            writer.WriteStartObject();
            writer.WritePropertyName("x");
            writer.WriteValue(vector3.x);
            writer.WritePropertyName("y");
            writer.WriteValue(vector3.y);
            writer.WritePropertyName("z");
            writer.WriteValue(vector3.z);
            writer.WriteEndObject();
        }
    }

    public class Vector2Converter : JsonConverter
    {
        public override bool CanConvert(System.Type objectType)
        {
            return objectType == typeof(Vector2);
        }

        public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer)
        {
            var value = serializer.Deserialize(reader);
            return JsonConvert.DeserializeObject<Vector2>(value.ToString());
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var vector3 = (Vector2)value;

            writer.WriteStartObject();
            writer.WritePropertyName("x");
            writer.WriteValue(vector3.x);
            writer.WritePropertyName("y");
            writer.WriteValue(vector3.y);
            writer.WriteEndObject();
        }
    }
}
