using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using HarmonyLib;
using ModestTree;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace SaberFactory.Helpers
{
    internal class BaseComponentModifier : IFactorySerializable
    {
        public Type ComponentType { get; }

        public string GoName { get; }

        public string TypeName { get; }
        public readonly int Index;

        private readonly List<(ComponentValueAttribute, PropertyInfo)> _updateProps = new List<(ComponentValueAttribute, PropertyInfo)>();

        protected BaseComponentModifier(Component component, int index)
        {
            Index = index;
            ComponentType = component.GetType();
            GoName = component.name;
            TypeName = ComponentType.Name;
            GetUpateProps();
        }

        public Task FromJson(JObject obj, Serializer serializer)
        {
            foreach (var prop in _updateProps)
            {
                var objTkn = obj[prop.Item2.Name];
                if (objTkn is { })
                {
                    prop.Item2.SetValue(this, objTkn.ToObject(prop.Item2.PropertyType));
                }
            }

            return Task.CompletedTask;
        }

        public Task<JToken> ToJson(Serializer serializer)
        {
            var obj = new JObject();
            foreach (var prop in _updateProps)
            {
                obj.Add(prop.Item2.Name, JToken.FromObject(prop.Item2.GetValue(this)));
            }

            return Task.FromResult<JToken>(obj);
        }

        public virtual void SetInstance(Component instanceComponent)
        {
            foreach (var prop in _updateProps)
            {
                if (prop.Item1.AutoInit)
                {
                    prop.Item2.SetValue(this, prop.Item2.GetValue(this));
                }
            }
        }

        private void GetUpateProps()
        {
            foreach (var property in AccessTools.GetDeclaredProperties(GetType()))
            {
                if (!property.HasAttribute(typeof(ComponentValueAttribute)))
                {
                    continue;
                }

                if (property.GetAttribute<ComponentValueAttribute>() is { } attr)
                {
                    _updateProps.Add((attr, property));
                }
            }
        }

        public virtual string DrawUi()
        {
            return "<div></div>";
        }

        public virtual void Sync(BaseComponentModifier otherMod)
        {
            foreach (var prop in _updateProps)
            {
                prop.Item2.SetValue(this, prop.Item2.GetValue(otherMod));
            }
        }

        internal class ComponentValueAttribute : Attribute
        {
            public bool AutoInit = true;
        }
    }

    internal class BaseComponentModifier<T> : BaseComponentModifier where T : Component
    {
        public T Component { get; private set; }

        protected BaseComponentModifier(Component component, int index) : base(component, index)
        {
            Init((T)component);
        }

        protected virtual void Init(T component)
        {
        }

        public override void SetInstance(Component instanceComponent)
        {
            Component = (T)instanceComponent;
            base.SetInstance(instanceComponent);
        }
    }
}