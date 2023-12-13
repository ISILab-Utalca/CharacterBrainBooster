using ArtificialIntelligence.Utility;
using CBB.Lib;
using Generic;
using Newtonsoft.Json;
using System;
using System.Collections;
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
        [SerializeField,JsonRequired]
        private string classType;

        [SerializeField, SerializeReference, JsonRequired]
        private List<WraperValue> values = new();

        [JsonIgnore]
        public Type ClassType { 
            get => Type.GetType(classType); 
            set => classType = value.ToString(); 
        }
        [JsonIgnore]
        public List<WraperValue> Values { get => values; private set => values = value; }

        public DataGeneric() { }

        public void Add(WraperValue wraper)
        {
            Values.Add(wraper);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">The name of the value</param>
        /// <returns>A <see cref="WraperValue"/>. <b>null</b> if not found</returns>
        public WraperValue FindValueByName(string name)
        {
            return Values.Find(x => x.name == name);
        }

        public string GetItemName()
        {
            return classType;
        }
    }

    [System.Serializable]
    public abstract class WraperValue : ICloneable, IDataItem
    {
        public string name;

        public abstract object Clone();

        public virtual string GetItemName() { return "DATA ITEM"; }

        public abstract object Getvalue();
        public abstract override string ToString();
    }

    [System.Serializable]
    public class WraperNumber : WraperValue
    {
        public float value;

        public override object Clone()
        {
            return new WraperNumber { name = name, value = value };
        }

        public override object Getvalue()
        {
            return value;
        }

        public override string ToString()
        {
            return value.ToString();
        }
    }

    [System.Serializable]
    public class WraperString : WraperValue
    {
        public string value;

        public override object Clone()
        {
            return new WraperString { name = name, value = value };
        }

        public override object Getvalue()
        {
            return value;
        }

        public override string ToString()
        {
            return value.ToString();
        }
    }

    [System.Serializable]
    public class WraperBoolean : WraperValue
    {
        public bool value;

        public override object Clone()
        {
            return new WraperBoolean { name = name, value = value };
        }

        public override object Getvalue()
        {
            return value;
        }

        public override string ToString()
        {
            return value.ToString();
        }
    }
}

[System.Serializable]
public class WrapperConsideration : WraperValue // necesairo (?)
{
    [SerializeReference]
    public ConsiderationConfiguration configuration;


    public override object Clone()
    {
        return new WrapperConsideration { name = name, configuration = configuration };
    }

    public override object Getvalue()
    {
        return configuration;
    }
    public override string GetItemName()
    {
        return configuration.name;
    }
    public override string ToString()
    {
        return configuration.ToString();
    }
}