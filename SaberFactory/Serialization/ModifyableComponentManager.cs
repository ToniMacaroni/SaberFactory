using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace SaberFactory.Helpers
{
    internal class ModifyableComponentManager : IFactorySerializable
    {
        private static readonly Dictionary<Type, Type> ComponentToModifier = new Dictionary<Type, Type>
        {
            { typeof(MeshRenderer), typeof(MeshRendererModifier) }
        };

        public readonly Dictionary<int, BaseComponentModifier> ComponentByIndex = new Dictionary<int, BaseComponentModifier>();

        private readonly GameObject _prefab;

        public ModifyableComponentManager(GameObject prefab)
        {
            _prefab = prefab;
        }

        public async Task FromJson(JObject obj, Serializer serializer)
        {
            foreach (var mod in GetAllModifiers())
            {
                var modTkn = obj[mod.Index.ToString()];
                await mod.FromJson((JObject)modTkn, serializer);
            }
        }

        public async Task<JToken> ToJson(Serializer serializer)
        {
            var obj = new JObject();
            foreach (var mod in GetAllModifiers())
            {
                obj.Add(mod.Index.ToString(), await mod.ToJson(serializer));
            }

            return obj;
        }

        /// <summary>
        ///     Call on Model
        /// </summary>
        /// <param name="gameObject"></param>
        public void Map()
        {
            ComponentByIndex.Clear();
            foreach (var comp in EnumerateComponents(_prefab))
            {
                var mod = (BaseComponentModifier)Activator.CreateInstance(comp.modType, comp.component, comp.index);
                ComponentByIndex.Add(comp.index, mod);
            }
        }

        /// <summary>
        ///     Call when spawning instance
        /// </summary>
        /// <param name="gameObject"></param>
        public void SetInstance(GameObject gameObject)
        {
            foreach (var comp in EnumerateComponents(gameObject))
            {
                if (ComponentByIndex.TryGetValue(comp.index, out var mod))
                {
                    mod.SetInstance(comp.component);
                }
            }
        }

        public void Sync(ModifyableComponentManager otherManager)
        {
            var otherMods = otherManager.GetAllModifiers().ToList();
            var mods = GetAllModifiers().ToList();
            for (var i = 0; i < mods.Count; i++)
            {
                mods[i].Sync(otherMods[i]);
            }
        }

        public IEnumerable<BaseComponentModifier> GetAllModifiers()
        {
            return ComponentByIndex.Values;
        }

        public void ResetAll()
        {
            Map();
        }

        public void Reset(int idx)
        {
            foreach (var comp in EnumerateComponents(_prefab))
            {
                if (comp.index != idx)
                {
                    continue;
                }

                var mod = (BaseComponentModifier)Activator.CreateInstance(comp.modType, comp.component, comp.index);
                ComponentByIndex[idx] = mod;
            }
        }

        private IEnumerable<(int index, Component component, Type modType)> EnumerateComponents(GameObject gameObject)
        {
            var components = gameObject.GetComponentsInChildren<Component>();
            for (var i = 0; i < components.Length; i++)
            {
                var component = components[i];
                if (ComponentToModifier.TryGetValue(component.GetType(), out var modType))
                {
                    yield return (i, component, modType);
                }
            }
        }
    }
}