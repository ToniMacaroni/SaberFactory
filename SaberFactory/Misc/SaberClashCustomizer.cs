using System.IO;
using HarmonyLib;
using IPA.Utilities;
using SaberFactory.Helpers;
using SaberFactory.Instances;
using SaberFactory.Models;
using SaberFactory.ProjectComponents;
using SiraUtil.Affinity;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace SaberFactory.Misc
{
    internal class PSManager
    {
        public readonly ParticleSystem ParticleSystem;
        public readonly ParticleSystemRenderer Renderer;

        private ParticleSystem.MainModule _main;

        public Material Material
        {
            get => Renderer.sharedMaterial;
            set => Renderer.sharedMaterial = value;
        }

        public Color Color
        {
            get => _main.startColor.color;
            set => _main.startColor = value;
        }

        public ParticleSystem.MainModule Main => _main;

        public PSManager(ParticleSystem particleSystem)
        {
            ParticleSystem = particleSystem;
            _main = particleSystem.main;
            Renderer = particleSystem.GetComponent<ParticleSystemRenderer>();
        }
    }

    internal class SaberClashCustomizer : IInitializable, IAffinity, ICustomizer
    {
        public bool ClashEnabled { get; set; }

        private SaberClashEffect _currentClashEffect;
        private PSManager _sparkle;
        private PSManager _glow;

        internal SaberClashCustomizer(EmbeddedAssetLoader assetLoader)
        {
        }

        public void SetSaber(SaberInstance saber)
        {
            var info = saber.Model.PieceCollection[AssetTypeDefinition.CustomSaber].ModelComposition
                .AdditionalInstanceHandler.GetComponent<SFClashEffect>();
            if (!info)
            {
                return;
            }

            if (info.Material)
            {
                _glow.Material = info.Material;
            }
        }

        [AffinityPostfix]
        [AffinityPatch(typeof(SaberClashEffect), nameof(SaberClashEffect.Start))]
        protected void Setup(SaberClashEffect __instance, ParticleSystem ____sparkleParticleSystem, ParticleSystem ____glowParticleSystem)
        {
            _currentClashEffect = __instance;
            _sparkle = new PSManager(____sparkleParticleSystem);
            _glow = new PSManager(____glowParticleSystem);
        }

        public void Initialize()
        {

        }
    }

    //internal class SaberBurnMarkCustomizer : IInitializable, IAffinity, ICustomizer
    //{
    //    private SaberBurnMarkSparkles _currentSpakles;
    //    private readonly CustomSaberBurnMarkArea _burnMarkArea;

    //    private PSManager _sparkle;
    //    private PSManager[] _burnMarks;

    //    private SFBurnmarks _currentInfo;

    //    public SaberBurnMarkCustomizer(CustomSaberBurnMarkArea burnMarkArea)
    //    {
    //        _burnMarkArea = burnMarkArea;
    //    }

    //    public void SetSaber(SaberInstance saber)
    //    {
    //        var info = saber.Model.PieceCollection[AssetTypeDefinition.CustomSaber].ModelComposition
    //            .AdditionalInstanceHandler.GetComponent<SFBurnmarks>();
    //        if (!info)
    //        {
    //            return;
    //        }

    //        if (info.BurnMarkMaterial)
    //        {
    //            _burnMarkArea.BurnmarkMaterial = info.BurnMarkMaterial;
    //        }

    //        if (info.SparkleMaterial)
    //        {
    //            _sparkle.Material = info.SparkleMaterial;
    //        }

    //        if (info.FadeoutMaterial)
    //        {
    //            _burnMarkArea.FadeoutMaterial = new Material(info.FadeoutMaterial);
    //        }

    //        if (info.FloorMaterial)
    //        {
    //            _burnMarkArea.GetComponent<Renderer>().material = new Material(info.FloorMaterial);
    //        }

    //        _burnMarkArea.BurnmarkSize = info.BurnmarkSize;

    //        _burnMarkArea.FadeoutStrength = info.FadeStrength;
    //        _burnMarkArea.RandomOffset = info.RandomBurnmarkJitter;
    //    }

    //    [AffinityPostfix]
    //    [AffinityPatch(typeof(SaberBurnMarkSparkles), nameof(SaberBurnMarkSparkles.Start))]
    //    protected void Setup(SaberBurnMarkSparkles __instance, ParticleSystem ____sparklesPS,
    //        ParticleSystem[] ____burnMarksPS)
    //    {
    //        _currentSpakles = __instance;

    //        _sparkle = new PSManager(____sparklesPS);
    //        Object.DestroyImmediate(_sparkle.Renderer);
    //        _burnMarks = new PSManager[2];
    //        _burnMarks[0] = new PSManager(____burnMarksPS[0]);
    //        _burnMarks[1] = new PSManager(____burnMarksPS[1]);

    //        Object.DestroyImmediate(_burnMarks[0].Renderer);
    //        Object.DestroyImmediate(_burnMarks[1].Renderer);
    //    }

    //    public void Initialize()
    //    {

    //    }
    //}

    internal interface ICustomizer
    {
        public void SetSaber(SaberInstance saber);
    }
}
