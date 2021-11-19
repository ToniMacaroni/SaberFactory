using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using HarmonyLib;
using ModestTree;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace SaberFactory.Serialization
{
    internal class UpdatableSerialializable : IFactorySerializable
    {
        private readonly List<(HandledValueAttribute, PropertyInfo)> _updateProps = new List<(HandledValueAttribute, PropertyInfo)>();

        protected UpdatableSerialializable()
        {
            GetUpateProps();
        }

        public Task FromJson(JObject obj, Serializer serializer)
        {
            if (obj == null)
            {
                return Task.CompletedTask;
            }

            foreach (var prop in _updateProps)
            {
                var objTkn = obj[prop.Item2.Name];
                if (objTkn is null)
                {
                    continue;
                }

                prop.Item2.SetValue(this, objTkn.ToObject(prop.Item2.PropertyType, Serializer.JsonSerializer));
            }

            return Task.CompletedTask;
        }

        public Task<JToken> ToJson(Serializer serializer)
        {
            var obj = new JObject();

            foreach (var prop in _updateProps)
            {
                obj.Add(prop.Item2.Name, JToken.FromObject(prop.Item2.GetValue(this), Serializer.JsonSerializer));
            }

            return Task.FromResult<JToken>(obj);
        }

        public virtual void Update()
        {
            foreach (var prop in _updateProps)
            {
                if (prop.Item1.Update)
                {
                    prop.Item2.SetValue(this, prop.Item2.GetValue(this));
                }
            }
        }

        private void GetUpateProps()
        {
            foreach (var property in AccessTools.GetDeclaredProperties(GetType()))
            {
                if (!property.HasAttribute(typeof(HandledValueAttribute)))
                {
                    continue;
                }

                if (property.GetAttribute<HandledValueAttribute>() is { } attr)
                {
                    _updateProps.Add((attr, property));
                }
            }
        }

        public virtual void Sync(object otherMod)
        {
            foreach (var prop in _updateProps)
            {
                prop.Item2.SetValue(this, prop.Item2.GetValue(otherMod));
            }
        }

        internal class HandledValueAttribute : Attribute
        {
            public bool Update = true;
        }
    }
}