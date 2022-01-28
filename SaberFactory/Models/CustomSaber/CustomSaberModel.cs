using System;
using System.Linq;
using System.Threading.Tasks;
using CustomSaber;
using Newtonsoft.Json.Linq;
using SaberFactory.DataStore;
using SaberFactory.Helpers;
using SaberFactory.Instances;
using SaberFactory.Instances.CustomSaber;
using SaberFactory.Models.PropHandler;
using SaberFactory.Serialization;
using UnityEngine;
using Zenject;

namespace SaberFactory.Models.CustomSaber
{
    public class CustomSaberModel : BasePieceModel
    {
        public override Type InstanceType { get; protected set; } = typeof(CustomSaberInstance);

        public TrailModel TrailModel
        {
            get
            {
                if (_trailModel == null)
                {
                    var trailModel = GrabTrail(false);
                    if (trailModel == null)
                    {
                        _hasTrail = false;
                        return null;
                    }

                    _trailModel = trailModel;
                }

                return _trailModel;
            }

            set => _trailModel = value;
        }

        public bool HasTrail
        {
            get
            {
                _hasTrail ??= Prefab != null && Prefab.GetComponent<CustomTrail>() != null;
                return _hasTrail.Value;
            }
        }

        public SaberDescriptor SaberDescriptor;
        private bool _didReparentTrail;
        private bool? _hasTrail;

        private TrailModel _trailModel;

        [Inject] private readonly PluginDirectories _pluginDirectories = null;

        public CustomSaberModel(StoreAsset storeAsset) : base(storeAsset)
        {
            PropertyBlock = new CustomSaberPropertyBlock();
        }

        public override void OnLazyInit()
        {
            if (!HasTrail)
            {
                return;
            }

            var trailModel = TrailModel;

            var path = _pluginDirectories.Cache.GetFile(StoreAsset.NameWithoutExtension+".trail").FullName;
            var trail = QuickSave.LoadObject<TrailProportions>(path);
            if (trail == null)
            {
                return;
            }

            trailModel.Length = trail.Length;
            trailModel.Width = trail.Width;
        }

        public override void SaveAdditionalData()
        {
            if (!HasTrail)
            {
                return;
            }

            var trailModel = TrailModel;

            var path = _pluginDirectories.Cache.GetFile(StoreAsset.NameWithoutExtension+".trail").FullName;
            var trail = new TrailProportions
            {
                Length = trailModel.Length,
                Width = trailModel.Width
            };
            QuickSave.SaveObject(trail, path);
        }

        public override ModelMetaData GetMetaData()
        {
            return new ModelMetaData(SaberDescriptor.SaberName, SaberDescriptor.AuthorName,
                SaberDescriptor.CoverImage, false);
        }

        public override void SyncFrom(BasePieceModel otherModel)
        {
            base.SyncFrom(otherModel);
            var otherCs = (CustomSaberModel)otherModel;

            if (otherCs.HasTrail || otherCs.TrailModel is { })
            {
                TrailModel ??= new TrailModel();

                TrailModel.TrailOriginTrails = otherCs.TrailModel.TrailOriginTrails;

                // backup current material
                var originalMaterial = TrailModel.Material?.Material;

                TrailModel.CopyFrom(otherCs.TrailModel);

                var otherMat = TrailModel.Material.Material;

                // if trail isn't from different saber just copy props
                // if trail IS from different saber but shares the same shader just copy props
                // otherwise (trail is from other saber and shaders are different) copy the whole material
                if (originalMaterial != null && (string.IsNullOrWhiteSpace(TrailModel.TrailOrigin) ||
                                    originalMaterial.shader.name == otherMat.shader.name))
                {
                    foreach (var prop in otherMat.GetProperties(MaterialAttributes.HideInSf))
                    {
                        originalMaterial.SetProperty(prop.Item2, prop.Item1, prop.Item3);
                    }

                    TrailModel.Material.Material = originalMaterial;
                }
                else
                {
                    originalMaterial.TryDestoryImmediate();
                }
            }
        }

        public TrailModel GrabTrail(bool addTrailOrigin)
        {
            var trail = SaberHelpers.GetTrails(Prefab).FirstOrDefault();

            if (trail == null)
            {
                return null;
            }

            TextureWrapMode wrapMode = default;
            if (trail.TrailMaterial != null && trail.TrailMaterial.TryGetMainTexture(out var tex))
            {
                wrapMode = tex.wrapMode;
            }

            FixTrailParents();

            return new TrailModel(
                Vector3.zero,
                trail.GetWidth(),
                trail.Length,
                new MaterialDescriptor(trail.TrailMaterial),
                0, wrapMode,
                addTrailOrigin ? StoreAsset.RelativePath : null);
        }

        /// <summary>
        ///     Resets the trail using the original <see cref="CustomTrail" /> component
        /// </summary>
        public void ResetTrail()
        {
            TrailModel = GrabTrail(false);
        }

        /// <summary>
        ///     Reparent trail transforms to specified parent
        ///     so we don't have to care about scaling and rotations afterwards
        /// </summary>
        /// <param name="parent">Transform to parent the trail transforms to</param>
        /// <param name="trail"></param>
        public void FixTrailParents()
        {
            if (_didReparentTrail)
            {
                return;
            }

            _didReparentTrail = true;

            var trail = Prefab.GetComponent<CustomTrail>();

            if (trail is null)
            {
                return;
            }

            trail.PointStart.SetParent(Prefab.transform, true);
            trail.PointEnd.SetParent(Prefab.transform, true);
        }

        public override async Task FromJson(JObject obj, Serializer serializer)
        {
            await base.FromJson(obj, serializer);
            var trailModelToken = obj[nameof(TrailModel)];
            if (trailModelToken != null)
            {
                if (TrailModel == null)
                {
                    TrailModel = new TrailModel();
                }

                await TrailModel.FromJson((JObject)trailModelToken, serializer);
            }
        }

        public override async Task<JToken> ToJson(Serializer serializer)
        {
            var obj = (JObject)await base.ToJson(serializer);
            
            if (TrailModel != null)
            {
                obj.Add(nameof(TrailModel), await TrailModel.ToJson(serializer));
            }

            return obj;
        }

        internal class Factory : PlaceholderFactory<StoreAsset, CustomSaberModel>
        { }

        internal class TrailProportions
        {
            public int Length;
            public float Width;
        }
    }
}