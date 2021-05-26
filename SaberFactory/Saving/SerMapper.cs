using System;
using System.Collections.Generic;
using System.Reflection;
using IPA.Config.Data;

namespace SaberFactory.Saving
{
    internal static class SerMapper
    {
        private static readonly FieldMapCollection FieldMaps = new FieldMapCollection();

        public static void Map(object from, object to)
        {
            var fromMap = GetFieldMap(from.GetType());
            var toMap = GetFieldMap(to.GetType());

            if (fromMap == null)
            {
                throw new ArgumentNullException(nameof(from.GetType), "FieldMap needs to be registered first");
            }

            if (toMap == null)
            {
                throw new ArgumentNullException(nameof(to.GetType), "FieldMap needs to be registered first");
            }

            foreach (var field in fromMap.Values)
            {
                if(!toMap.TryGetValue(field.Name, out var otherField)) continue;
                var val = field.GetValue(from);
                otherField.SetValue(to, val);
            }
        }

        public static void CreateEntry(Type from, Type to)
        {
            if (FieldMaps.TryGetValue(from, out _)) return;
            if (FieldMaps.TryGetValue(to, out _)) return;

            var fromMap = new FieldMap();
            var toMap = new FieldMap();

            var fields = GetFields(from);
            foreach (var field in fields)
            {
                fromMap.Add(field);
                var otherField = to.GetField(field.Name);
                if(otherField != null) toMap.Add(otherField);
            }

            FieldMaps.Add(from, fromMap);
            FieldMaps.Add(to, toMap);
        }

        public static void CreateEntry<TFrom, TTo>()
        {
            CreateEntry(typeof(TFrom), typeof(TTo));
        }

        private static List<FieldInfo> GetFields(Type type)
        {
            var fields = new List<FieldInfo>();

            foreach (var fieldInfo in type.GetFields())
            {
                var attr = fieldInfo.GetCustomAttribute<MapSerializeAttribute>();
                if(attr != null) fields.Add(fieldInfo);
            }

            return fields;
        }

        private static FieldMap GetFieldMap(Type type)
        {
            if (FieldMaps.TryGetValue(type, out var fieldMap)) return fieldMap;
            return null;
        }

        internal class FieldMapCollection : Dictionary<Type, FieldMap>
        {
        }

        internal class FieldMap : Dictionary<string, FieldInfo>
        {
            public void Add(FieldInfo field)
            {
                Add(field.Name, field);
            }
        }
    }
}