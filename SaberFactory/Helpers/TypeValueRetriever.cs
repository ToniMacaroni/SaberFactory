using System;
using System.Reflection;

namespace SaberFactory.Helpers
{
    public class TypeValueRetriever
    {
        private Type _type;
        private object _instance;

        public TypeValueRetriever(Type type, object instance)
        {
            _type = type;
            _instance = instance;
        }

        public T GetValue<T>(string name)
        {
            FieldInfo field = _type.GetField(name);
            return (T)field?.GetValue(_instance);
        }
    }
}