using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomSaber;
using ModestTree;
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
            get => _trailModel ??= GrabTrail(false);
            set => _trailModel = value;
        }

        public bool HasTrail => TrailModel != null;

        public bool HasNativeTrail => FirstNativeTrail != null;

        public CustomTrail FirstNativeTrail => NativeTrails.FirstOrDefault();

        public List<CustomTrail> NativeTrails => _nativeTrails.Value;

        public SaberDescriptor SaberDescriptor;
        private bool _didReparentTrail;

        private readonly Lazy<List<CustomTrail>> _nativeTrails;

        private TrailModel _trailModel;

        [Inject] private readonly PluginDirectories _pluginDirectories = null;

        public CustomSaberModel(StoreAsset storeAsset) : base(storeAsset)
        {
            _nativeTrails = new Lazy<List<CustomTrail>>(NativeTrailValueFactory);
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

            trailModel.Length.SetWithoutInvoke(trail.Length);
            trailModel.Width.SetWithoutInvoke(trail.Width);
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
                Length = trailModel.Length.Value,
                Width = trailModel.Width.Value
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
                    foreach (var prop in otherMat.GetProperties(MaterialAttributes.SFHide))
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

        /// <summary>
        /// Find trail on this saber and return a new trail model
        /// </summary>
        /// <param name="addTrailOrigin">add this asset to the trail information</param>
        /// <param name="replaceMat">if provided, will replace the current trail material with this one</param>
        /// <returns></returns>
        public TrailModel GrabTrail(bool addTrailOrigin, Material replaceMat = null)
        {
            var trail = FirstNativeTrail;

            if (!trail)
            {
                Debug.Log($"{Prefab.name} doesn't have a trail");
                return null;
            }

            if (replaceMat)
            {
                trail.TrailMaterial = replaceMat;
            }
            
            // Trail without material doesn't do anything
            // so treat it as if there is no trail
            if (!trail.TrailMaterial)
            {
                Debug.Log($"{Prefab.name} trail has no material");
                return null;
            }

            // if material and tex exists, get the wrap mode
            TextureWrapMode wrapMode = default;
            if (trail.TrailMaterial && trail.TrailMaterial.TryGetMainTexture(out var tex))
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
                addTrailOrigin ? StoreAsset.RelativePath.Path : null);
        }

        /// <summary>
        ///     Resets the trail using the original <see cref="CustomTrail" /> component
        /// </summary>
        public void ResetTrail()
        {
            if (TrailModel == null)
            {
                return;
            }

            if (!TrailModel.Material.IsValid)
            {
                Debug.Log($"{nameof(TrailModel)} material is invalid");
                return;
            }

            TrailModel.Material.Revert();

            TrailModel = GrabTrail(false, TrailModel.Material.Material);
        }

        /// <summary>
        ///     Reparent trail transforms to specified parent
        ///     so we don't have to care about scaling and rotations afterwards
        /// </summary>
        public void FixTrailParents()
        {
            if (_didReparentTrail)
            {
                return;
            }

            _didReparentTrail = true;

            var trail = FirstNativeTrail;

            if (!trail)
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
        
        private List<CustomTrail> NativeTrailValueFactory()
        {
            return SaberHelpers.GetTrails(Prefab);
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