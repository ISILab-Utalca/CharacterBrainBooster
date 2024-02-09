using CBB.Api;
using Generic;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// this class is used to store the generic brain data
/// </summary>
[System.Serializable]
public class Brain : IDataItem, INameable
{
    public string brain_ID;
    public string brain_Name;
    [SerializeField, SerializeReference]
    public List<DataGeneric> serializedActions;
    [SerializeField, SerializeReference]
    public List<DataGeneric> serializedSensors;
    public Brain(string brain_ID, string brain_Name, List<DataGeneric> serializedActions, List<DataGeneric> serializedSensors)
    {
        this.brain_ID = brain_ID;
        this.brain_Name = brain_Name;
        this.serializedActions = serializedActions;
        this.serializedSensors = serializedSensors;
    }
    
    public Brain()
    {
        this.brain_ID = "";
        this.brain_Name = "";
        this.serializedActions = new List<DataGeneric>();
        this.serializedSensors = new List<DataGeneric>();
    }
    public object GetInstance() => this;
    public string GetItemName() => brain_Name;
    public void SetName(string name) => brain_Name = name;
}



