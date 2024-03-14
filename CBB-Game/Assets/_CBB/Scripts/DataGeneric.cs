using CBB.Lib;
using Generic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Generic
{
    public interface IGeneric
    {
        public void SetParams(DataGeneric data);

        public DataGeneric GetGeneric();
    }

    [System.Serializable]
    public class DataGeneric : IDataItem
    {
        public enum DataType
        {
            Action,
            Curve,
            Sensor,
            Default
        }
        [SerializeField,JsonRequired]
        private string classType;

        [SerializeField, SerializeReference, JsonRequired]
        private List<WraperValue> values = new();

        [SerializeField, JsonRequired]
        private DataType dataType = DataType.Default;
        [JsonIgnore]
        public Type ClassType { 
            get => Type.GetType(classType); 
            set => classType = value.ToString(); 
        }
        [JsonIgnore]
        public List<WraperValue> Values { get => values; private set => values = value; }

        public DataGeneric() { }
        public DataGeneric(DataType dataType) : this()
        {
            this.dataType = dataType;
        }
        public void Add(WraperValue wraper) => Values.Add(wraper);
        public WraperValue FindValueByName(string name) => Values.Find(x => x.name == name);
        public string GetItemName() => classType;
        public object GetInstance() => this;
        public DataType GetDataType() => dataType;
    }

    [System.Serializable]
    public abstract class WraperValue : ICloneable, IDataItem
    {
        public string name;

        public abstract object Clone();

        public virtual string GetItemName() => "DATA ITEM";

        public abstract object Getvalue();
        public abstract override string ToString();
        public object GetInstance() => this;
    }

    [System.Serializable]
    public class WraperNumber : WraperValue
    {
        public float value;

        public override object Clone() => new WraperNumber { name = name, value = value };

        public override object Getvalue() => value;

        public override string ToString() => value.ToString();
    }

    [System.Serializable]
    public class WraperString : WraperValue
    {
        public string value;

        public override object Clone() => new WraperString { name = name, value = value };

        public override object Getvalue() => value;

        public override string ToString() => value.ToString();
    }

    [System.Serializable]
    public class WraperBoolean : WraperValue
    {
        public bool value;

        public override object Clone() => new WraperBoolean { name = name, value = value };

        public override object Getvalue() => value;

        public override string ToString() => value.ToString();
    }
}

[System.Serializable]
public class WrapperConsideration : WraperValue
{
    //NOTE: since storing a reference to a UtilityConsideration (ScriptableObject) had issues,
    //we store a reference to a ConsiderationConfiguration object instead, which holds all the
    //data we need to recreate/update the UtilityConsideration object
    [SerializeReference]
    public ConsiderationConfiguration configuration;


    public override object Clone() => new WrapperConsideration { name = name, configuration = configuration };

    public override object Getvalue() => configuration;
    public override string GetItemName() => configuration.considerationName;
    public override string ToString() => configuration.ToString();
}