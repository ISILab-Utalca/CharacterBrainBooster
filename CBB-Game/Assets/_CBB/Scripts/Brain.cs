using CBB.Api;
using CBB.Comunication;
using Generic;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// this class is used to store the generic brain data
/// </summary>
[System.Serializable]
public class Brain : IDataItem, INameable
{
    public string id;
    public string name;
    [SerializeField, SerializeReference]
    public List<DataGeneric> serializedActions;
    [SerializeField, SerializeReference]
    public List<DataGeneric> serializedSensors;
    public Brain(string id, string name, List<DataGeneric> serializedActions, List<DataGeneric> serializedSensors)
    {
        this.id = id;
        this.name = name;
        this.serializedActions = serializedActions;
        this.serializedSensors = serializedSensors;
    }
    
    public Brain()
    {
        id = "";
        name = "";
        serializedActions = new List<DataGeneric>();
        serializedSensors = new List<DataGeneric>();
    }
    public BrainIdentification GetBrainIdentification() => new(id, name);
    public object GetInstance() => this;
    public string GetItemName() => name;
    public void SetName(string name) => this.name = name;
}



