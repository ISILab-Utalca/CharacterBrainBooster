using CBB.Lib;
using Newtonsoft.Json.Serialization;
using System;
using UnityEngine;
/// <summary>
/// Custom binder for every SensorData class
/// </summary>
public class GeneralBinder : ISerializationBinder
{
    private readonly Type bindType;
    public GeneralBinder(Type bindType)
    {
        this.bindType = bindType;
    }
    public void BindToName(Type serializedType, out string assemblyName, out string typeName)
    {
        throw new NotImplementedException();
    }

    Type ISerializationBinder.BindToType(string assemblyName, string typeName)
    {
        Debug.Log($"Assembly name argument: {assemblyName}");
        Debug.Log($"Assembly of Agent Basic Data: {bindType.Assembly.GetName().Name}");
        if (assemblyName != bindType.Assembly.GetName().Name)
        {
            Debug.Log("Assemblies don't match");

            return null;
            throw new TypeLoadException($"Error on BindToType: assembly names of {bindType.Assembly.FullName} and {assemblyName} don't match");
        }
        Debug.Log($"type name argument: {typeName}");
        Debug.Log($"Full Type name of Sensor Data: {bindType.FullName}");
        if (typeName != bindType.FullName)
        {
            Debug.Log("types name don't match");
            return null;
            throw new TypeLoadException($"Error on BindToType: assembly type of {bindType.Name} and {typeName} don't match");
        }
        return bindType;
    }
}