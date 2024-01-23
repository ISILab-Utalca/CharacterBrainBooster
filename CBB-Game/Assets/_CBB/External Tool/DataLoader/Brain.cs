using Generic;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// this class is used to store the generic brain data
/// </summary>
[System.Serializable]
public class Brain : IDataItem
{
    public string brain_ID;
    [SerializeField, SerializeReference]
    public List<DataGeneric> serializedActions;
    [SerializeField, SerializeReference]
    public List<DataGeneric> serializedSensors;

    public object GetInstance() => this;

    public string GetItemName() => brain_ID;
    
}



