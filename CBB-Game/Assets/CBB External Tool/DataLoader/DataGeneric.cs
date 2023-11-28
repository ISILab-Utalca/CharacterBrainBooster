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
    public class DataGeneric
    {
        private string classType;

        [SerializeReference]
        private List<WraperValue> values = new List<WraperValue>();

        [JsonIgnore]
        public Type ClassType { get => Type.GetType(classType); }

        public DataGeneric(Type type)
        {
            classType = type.ToString();
        }

        public void Add(WraperValue wraper)
        {
            values.Add(wraper);
        }

        public WraperValue Get(string name)
        {
            return values.Find(x => x.name == name);
        }
    }

    [System.Serializable]
    public abstract class WraperValue : ICloneable
    {
        public string name;

        public abstract object Clone();
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