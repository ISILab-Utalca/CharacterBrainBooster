using CBB.Lib;
using Newtonsoft.Json.Serialization;
using System;
using UnityEngine;
/// <summary>
/// Custom binder for every SensorData class
/// </summary>
public class SensorDataBinder : DefaultSerializationBinder
{
    public override Type BindToType(string assemblyName, string typeName)
    {
        Debug.Log($"Assembly name argument: {assemblyName}");
        Debug.Log($"Assembly of Sensor Data: {typeof(SensorData).Assembly.GetName().Name}");
        if (assemblyName != typeof(SensorData).Assembly.GetName().Name)
        {
            Debug.Log("Assemblies don't match");

            return null;
            throw new TypeLoadException($"Error on BindToType: assembly names of {typeof(SensorData).Assembly.FullName} and {assemblyName} don't match");
        }
        Debug.Log($"type name argument: {typeName}");
        Debug.Log($"Full Type name of Sensor Data: {typeof(SensorData).FullName}");
        if (typeName != typeof(SensorData).FullName)
        {
            Debug.Log("types name don't match");
            return null;
            throw new TypeLoadException($"Error on BindToType: assembly type of {typeof(SensorData).Name} and {typeName} don't match");
        }
        return typeof(SensorData);
    }
}

public class DummySimpleDataBinder : DefaultSerializationBinder
{
    public override Type BindToType(string assemblyName, string typeName)
    {
        Debug.Log($"Assembly name argument: {assemblyName}");
        Debug.Log($"Assembly of DummySimpleData: {typeof(DummySimpleData).Assembly.GetName().Name}");
        if (assemblyName != typeof(DummySimpleData).Assembly.GetName().Name)
        {
            Debug.Log("Assemblies don't match");

            return null;
            throw new TypeLoadException($"Error on BindToType: assembly names of {typeof(DummySimpleData).Assembly.FullName} and {assemblyName} don't match");
        }
        Debug.Log($"type name argument: {typeName}");
        Debug.Log($"Full Type name of Dummy Sensor: {typeof(DummySimpleData).FullName}");
        if (typeName != typeof(DummySimpleData).FullName)
        {
            Debug.Log("types name don't match");
            return null;
            throw new TypeLoadException($"Error on BindToType: assembly type of {typeof(DummySimpleData).Name} and {typeName} don't match");
        }
        return typeof(DummySimpleData);
    }
}
public class AgentBasicDataBinder : DefaultSerializationBinder
{
    public override Type BindToType(string assemblyName, string typeName)
    {
        Debug.Log($"Assembly name argument: {assemblyName}");
        Debug.Log($"Assembly of Agent Basic Data: {typeof(AgentBasicData).Assembly.GetName().Name}");
        if (assemblyName != typeof(AgentBasicData).Assembly.GetName().Name)
        {
            Debug.Log("Assemblies don't match");

            return null;
            throw new TypeLoadException($"Error on BindToType: assembly names of {typeof(AgentBasicData).Assembly.FullName} and {assemblyName} don't match");
        }
        Debug.Log($"type name argument: {typeName}");
        Debug.Log($"Full Type name of Sensor Data: {typeof(AgentBasicData).FullName}");
        if (typeName != typeof(AgentBasicData).FullName)
        {
            Debug.Log("types name don't match");
            return null;
            throw new TypeLoadException($"Error on BindToType: assembly type of {typeof(AgentBasicData).Name} and {typeName} don't match");
        }
        return typeof(AgentBasicData);
    }
}