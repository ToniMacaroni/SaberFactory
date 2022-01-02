using System.Collections.Generic;
using System.Threading.Tasks;
using CustomSaber;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SaberFactory.Helpers;
using SaberFactory.Instances;
using SaberFactory.Models.CustomSaber;
using SaberFactory.Serialization;
using UnityEngine;

namespace SaberFactory.Models
{
    /// <summary>
    ///     Stores information on how to build a trail
    /// </summary>
    public class TrailModel : IFactorySerializable
    {
        public int OriginalLength { get; private set; }

        [MapSerialize] public bool ClampTexture;

        [MapSerialize] public bool Flip;

        [MapSerialize] public int Length;

        [JsonIgnore] public MaterialDescriptor Material;

        public TextureWrapMode? OriginalTextureWrapMode;

        [MapSerialize] public string TrailOrigin;

        // for custom sabers with multiple trails
        [JsonIgnore] public List<CustomTrail> TrailOriginTrails;

        [MapSerialize] public Vector3 TrailPosOffset;

        [MapSerialize] public float Whitestep;

        [MapSerialize] public float Width;

        public TrailModel(
            Vector3 trailPosOffset,
            float width,
            int length,
            MaterialDescriptor material,
            float whitestep,
            TextureWrapMode? originalTextureWrapMode,
            string trailOrigin = "")
        {
            TrailPosOffset = trailPosOffset;
            Width = width;
            Length = length;
            OriginalLength = length;
            Material = material;
            Whitestep = whitestep;
            OriginalTextureWrapMode = originalTextureWrapMode;
            TrailOrigin = trailOrigin;
        }

        public TrailModel()
        { }

        public async Task FromJson(JObject obj, Serializer serializer)
        {
            obj.Populate(this);
            if (!string.IsNullOrEmpty(TrailOrigin))
            {
                await LoadFromTrailOrigin(serializer, TrailOrigin);
            }

            if (obj.SelectToken("Material") is { } materialToken)
            {
                if (Material is null)
                {
                    Material = new MaterialDescriptor(null);
                }

                await serializer.LoadMaterial((JObject)materialToken, Material.Material);
            }
        }

        public Task<JToken> ToJson(Serializer serializer)
        {
            var obj = JObject.FromObject(this, Serializer.JsonSerializer);
            
            if (Material.IsValid)
            {
                obj.Add("Material", serializer.SerializeMaterial(Material.Material));
            }

            return Task.FromResult<JToken>(obj);
        }

        public void CopyFrom(TrailModel other)
        {
            TrailPosOffset = other.TrailPosOffset;
            Width = other.Width;
            Length = other.Length;
            Material ??= new MaterialDescriptor(null);
            Material.Material = new Material(other.Material.Material);
            Whitestep = other.Whitestep;
            TrailOrigin = other.TrailOrigin;
            ClampTexture = other.ClampTexture;
            Flip = other.Flip;
            OriginalLength = other.OriginalLength;
        }

        private async Task LoadFromTrailOrigin(Serializer serializer, JToken trailOrigin)
        {
            var comp = await serializer.LoadPiece(trailOrigin);
            if (!(comp?.GetLeft() is CustomSaberModel cs))
            {
                return;
            }

            var originTrailModel = cs.GrabTrail(true);
            if (originTrailModel == null)
            {
                return;
            }

            Material ??= new MaterialDescriptor(null);
            Material.Material = originTrailModel.Material.Material;
            TrailOriginTrails = SaberHelpers.GetTrails(cs.Prefab);
        }
    }
}