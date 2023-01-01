using System.Collections.Generic;
using System.Threading.Tasks;
using CustomSaber;
using IPA.Config.Stores.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SaberFactory.AssetProperties;
using SaberFactory.Helpers;
using SaberFactory.Instances;
using SaberFactory.Misc;
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
        [JsonIgnore] public MaterialDescriptor Material;
        public TextureWrapMode? OriginalTextureWrapMode;
        public string TrailOrigin;
        [JsonIgnore] public List<CustomTrail> TrailOriginTrails;
        public Vector3 TrailPosOffset;

        public readonly IntProperty Length;
        public readonly FloatProperty Whitestep;
        public readonly FloatProperty Offset; // Vertical offset of the whole trail
        public readonly FloatProperty Width;
        public readonly BoolProperty ClampTexture; // Clamp the main texture of the trail
        public readonly BoolProperty Flip; // Flip the trail

        [JsonIgnore] private readonly ValueRange<int> _lengthRange = new(1, 30);
        [JsonIgnore] private readonly ValueRange<float> _widthRange = new(0.1f, 1.5f);
        [JsonIgnore] private readonly ValueRange<float> _offsetRange = new(-0.5f, 0.5f);

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
            Offset = new FloatProperty(trailPosOffset.z, _offsetRange);
            Width = new FloatProperty(width, _widthRange);
            Length = new IntProperty(length, _lengthRange);
            Material = material;
            Whitestep = new FloatProperty(whitestep);
            OriginalTextureWrapMode = originalTextureWrapMode;
            ClampTexture = new BoolProperty(originalTextureWrapMode == TextureWrapMode.Clamp);
            Flip = new BoolProperty(false);
            TrailOrigin = trailOrigin;
        }

        public TrailModel()
        {
            Length = new IntProperty(1, _lengthRange);
            Whitestep = new FloatProperty(0);
            Offset = new FloatProperty(0, _offsetRange);
            Width = new FloatProperty(1, _widthRange);
            ClampTexture = new BoolProperty(false);
            Flip = new BoolProperty(false);
        }

        public async Task FromJson(JObject obj, Serializer serializer)
        {
            obj.Populate(this);
            
            if (!string.IsNullOrEmpty(TrailOrigin))
            {
                await LoadFromTrailOrigin(serializer, TrailOrigin);
            }

            if (obj.SelectToken("Material") is { } materialToken)
            {
                Material ??= new MaterialDescriptor(null);

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
            Width.CopyFrom(other.Width);
            Length.CopyFrom(other.Length, true);
            Material ??= new MaterialDescriptor(null);
            Material.Material = new Material(other.Material.Material);
            Whitestep.CopyFrom(other.Whitestep);
            TrailOrigin = other.TrailOrigin;
            ClampTexture.CopyFrom(other.ClampTexture);
            Flip.CopyFrom(other.Flip);
        }

        private async Task LoadFromTrailOrigin(Serializer serializer, JToken trailOrigin)
        {
            var comp = await serializer.LoadPiece(trailOrigin);
            if (comp?.GetLeft() is not CustomSaberModel cs)
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
            TrailOriginTrails = cs.NativeTrails;
        }
    }
}