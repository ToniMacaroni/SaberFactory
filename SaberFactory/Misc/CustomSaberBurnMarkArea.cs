using System;
using SaberFactory.Configuration;
using SaberFactory.Models;
using SaberFactory.ProjectComponents;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace SaberFactory.Misc
{
    internal class CustomSaberBurnMarkArea : SaberBurnMarkArea
    {
        public Material FadeoutMaterial
        {
            get => _fadeOutMaterial;
            set
            {
                if (_fadeOutMaterial)
                {
                    DestroyImmediate(_fadeOutMaterial);
                    _fadeOutMaterial = value;
                }
            }
        }

        public float RandomOffset
        {
            get => _blackMarkLineRandomOffset;
            set => _blackMarkLineRandomOffset = value;
        }

        public float FadeoutStrength
        {
            get => _burnMarksFadeOutStrength;
            set => _burnMarksFadeOutStrength = value;
        }

        public float BurnmarkSize
        {
            get => _burnmarkSize;
            set
            {
                _burnmarkSize = value;
                foreach (var lineRenderer in _lineRenderers)
                {
                    lineRenderer.widthMultiplier = _burnmarkSize;
                }
            }
        }

        public Material BurnmarkMaterial
        {
            get => _lineRenderers[0].sharedMaterial;
            set
            {
                foreach (var lineRenderer in _lineRenderers)
                {
                    var oldMat = lineRenderer.sharedMaterial;
                    lineRenderer.material = new Material(value) {color = oldMat.color};
                }
            }
        }

        private float _burnmarkSize = 0.1f;

        [Inject] private readonly SaberSet _saberSet = null;
        [Inject] private readonly PluginConfig _pluginConfig = null;

        private SFBurnmarks _info;

        public override void Start()
        {
            base.Start();
            Init();
        }

        public void Init()
        {
            if (!_pluginConfig.EnableCustomBurnmarks)
            {
                return;
            }

            if (!_saberSet.LeftSaber.GetCustomSaber(out var cs))
            {
                return;
            }

            _info = cs.ModelComposition.AdditionalInstanceHandler.GetComponent<SFBurnmarks>();

            if (!_info)
            {
                return;
            }

            if (_info.BurnMarkMaterial)
            {
                BurnmarkMaterial = _info.BurnMarkMaterial;
            }

            //if (_info.SparkleMaterial)
            //{
            //    _sparkle.Material = _info.SparkleMaterial;
            //}

            if (_info.FadeoutMaterial)
            {
                FadeoutMaterial = new Material(_info.FadeoutMaterial);
            }

            if (_info.FloorMaterial)
            {
                GetComponent<Renderer>().material = new Material(_info.FloorMaterial);
            }

            BurnmarkSize = _info.BurnmarkSize;

            FadeoutStrength = _info.FadeStrength;
            RandomOffset = _info.RandomBurnmarkJitter;
        }
    }
}