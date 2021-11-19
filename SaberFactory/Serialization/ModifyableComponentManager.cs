using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SaberFactory.ProjectComponents;
using HarmonyLib;
using ModestTree;
using Newtonsoft.Json.Linq;
using SaberFactory.Modifiers;
using UnityEngine;

namespace SaberFactory.Serialization
{
    internal class ModifyableComponentManager : IFactorySerializable
    {
        public readonly Dictionary<int, BaseModifierImpl> Mods = new Dictionary<int, BaseModifierImpl>();
        
        public readonly SaberModifierCollection SaberModifierCollection;

        public bool IsAvailable => SaberModifierCollection != null;
        
        public ModifyableComponentManager(GameObject prefab)
        {
            SaberModifierCollection = prefab.GetComponent<SaberModifierCollection>();
        }

        public async Task FromJson(JObject obj, Serializer serializer)
        {
            if (!IsAvailable || obj == null || !obj["valid"].ToObject<bool>())
            {
                return;
            }

            var modsToken = obj["Mods"];
            if (modsToken != null)
            {
                foreach (var mod in Mods)
                {
                    var modTkn = modsToken[mod.Key.ToString()];
                    await mod.Value.FromJson((JObject)modTkn, serializer);
                }
            }
        }

        public async Task<JToken> ToJson(Serializer serializer)
        {
            if (!IsAvailable)
            {
                return new JObject { { "valid", false } };
            }

            var obj = new JObject();
            
            obj.Add("valid", true);

            var modsToken = new JObject();
            foreach (var mod in Mods)
            {
                modsToken.Add(mod.Key.ToString(), await mod.Value.ToJson(serializer));
            }

            obj.Add("Mods", modsToken);

            return obj;
        }

        /// <summary>
        ///     Call on Model
        /// </summary>
        /// <param name="gameObject"></param>
        public void Map()
        {
            if (!IsAvailable)
            {
                return;
            }

            SaberModifierCollection.Init();

            foreach (var vizMod in SaberModifierCollection.VisibilityModifiers)
            {
                var mod = new VisibilityModifierImpl(vizMod);
                Mods.Add(mod.Id, mod);
            }
            
            // for (var index = 0; index < SaberModifierCollection.ComponentModifiers.Length; index++)
            // {
            //     var mod = new VisibilityModifierImpl(SaberModifierCollection.VisibilityModifiers[index]);
            //     VisibilityModifiers.Add(index, mod);
            // }
            
            foreach (var tMod in SaberModifierCollection.TransformModifiers)
            {
                var mod = new TransformModifierImpl(tMod);
                Mods.Add(mod.Id, mod);
            }
        }

        /// <summary>
        ///     Call when spawning instance
        /// </summary>
        /// <param name="gameObject"></param>
        public void SetInstance(GameObject gameObject)
        {
            if (!IsAvailable)
            {
                return;
            }

            var saberModifierCollection = gameObject.GetComponent<SaberModifierCollection>();
            if (saberModifierCollection == null)
            {
                return;
            }

            if (!saberModifierCollection.Init())
            {
                Debug.LogWarning($"[{nameof(SaberModifierCollection)}] Init wasn't successfull");
                return;
            }

            foreach (var vizMod in saberModifierCollection.VisibilityModifiers)
            {
                if (Mods.TryGetValue(vizMod.Id, out var mod))
                {
                    mod.SetInstance(vizMod);
                }
            }
            
            foreach (var tMod in saberModifierCollection.TransformModifiers)
            {
                if (Mods.TryGetValue(tMod.Id, out var mod))
                {
                    mod.SetInstance(tMod);
                }
            }
        }

        public void Sync(ModifyableComponentManager otherManager)
        {
            if (!IsAvailable)
            {
                return;
            }
            
            var thisMods = GetAllMods();
            var otherMods = otherManager.GetAllMods();
            
            foreach (var otherMod in otherMods)
            {
                var thisMod = thisMods.FirstOrDefault(x => x.Name == otherMod.Name);
                if (thisMod == null)
                {
                    continue;
                }
                thisMod.Sync(otherMod);
            }
        }

        public List<BaseModifierImpl> GetAllMods()
        {
            return Mods.Values.ToList();
        }

        public void Reset(int id)
        {
            if (Mods.TryGetValue(id, out var mod))
            {
                mod.Reset();
            }
        }

        public void ResetAll()
        {
            foreach (var mod in Mods.Values)
            {
                mod.Reset();
            }
        }
    }
}