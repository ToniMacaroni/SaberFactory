using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HarmonyLib;
using IPA.Utilities;
using SaberFactory.Configuration;
using SaberFactory.Helpers;
using SaberFactory.Instances;
using SaberFactory.Misc;
using SaberFactory.Models;
using SiraUtil.Interfaces;
using SiraUtil.Logging;
using SiraUtil.Tools;
using UnityEngine;
using Zenject;

namespace SaberFactory.Game
{
    internal class SfSaberModelController : SaberModelController, IColorable
    {
        [InjectOptional] private readonly EventPlayer _eventPlayer = null;
        [Inject] private readonly GameSaberSetup _gameSaberSetup = null;
        [Inject] private readonly SiraLog _logger = null;
        [Inject] private readonly SaberInstance.Factory _saberInstanceFactory = null;
        [Inject] private readonly SaberSet _saberSet = null;
        [Inject] private readonly List<ICustomizer> _customizers = null;
        [Inject] private readonly PluginConfig _pluginConfig = null;
        private Color? _saberColor;

        private SaberInstance _saberInstance;

        public void SetColor(Color color)
        {
            _saberColor = color;
            _saberInstance.SetColor(color);
        }

        public Color Color
        {
            get => _saberColor.GetValueOrDefault();
            set => SetColor(value);
        }

        public override async void Init(Transform parent, Saber saber)
        {

            await _gameSaberSetup.SetupTask;

            transform.SetParent(parent, false);

            var saberModel = saber.saberType == SaberType.SaberA ? _saberSet.LeftSaber : _saberSet.RightSaber;

            _saberInstance = _saberInstanceFactory.Create(saberModel);

            if (saber.saberType == SaberType.SaberA)
            {
                _customizers.Do(x=>x.SetSaber(_saberInstance));
            }

            _saberInstance.SetParent(transform);
            _saberInstance.CreateTrail(false, _saberTrail); 
            SetColor(_saberColor ?? _colorManager.ColorForSaberType(_saberInstance.Model.SaberSlot.ToSaberType()));

            _eventPlayer?.SetPartEventList(_saberInstance.Events, saber.saberType);

            _logger.Info("Instantiated Saber");

            if (_pluginConfig.HMDOnly)
            {
                var defaultSaber = CreateDefaultSaberObject(parent, saber);
                CameraUtils.Core.VisibilityUtils.SetLayerRecursively(transform, CameraUtils.Core.VisibilityLayer.HmdOnly);
                _saberInstance.TrailHandler.SetVisibilityLayer(CameraUtils.Core.VisibilityLayer.HmdOnly);
                CameraUtils.Core.VisibilityUtils.SetLayerRecursively(defaultSaber, CameraUtils.Core.VisibilityLayer.DesktopOnlyAndReflected);
                CameraUtils.Core.VisibilityUtils.SetLayerRecursively(defaultSaber.GetComponent<SaberTrail>().GetField<SaberTrailRenderer, SaberTrail>("_trailRenderer").gameObject, CameraUtils.Core.VisibilityLayer.DesktopOnlyAndReflected);
            }
            else
            {
                CameraUtils.Core.VisibilityUtils.SetLayerRecursively(transform, CameraUtils.Core.VisibilityLayer.Saber);
                _saberInstance.TrailHandler.SetVisibilityLayer(CameraUtils.Core.VisibilityLayer.Saber);
            }

        }

        GameObject CreateDefaultSaberObject(Transform parent, Saber saber)
        {
            var baseVisualsGO = GameObject.Instantiate(HMDOnlySaber._defaultSaberPrefab, parent, false);
            var defaultSaberModelCon = baseVisualsGO.GetComponent<SaberModelController>();
            defaultSaberModelCon.SetField("_colorManager", this._colorManager);
            defaultSaberModelCon.SetField("_initData", this._initData);
            defaultSaberModelCon.SetField("_saberLight", this._saberLight);
            SetSaberGlowColor[] setSaberGlowColors = defaultSaberModelCon.GetField<SetSaberGlowColor[], SaberModelController>("_setSaberGlowColors");
            for (int i = 0; i < setSaberGlowColors.Length; i++)
            {
                setSaberGlowColors[i].SetField("_colorManager", this._colorManager);
            }
            SetSaberFakeGlowColor[] setSaberFakeGlowColors = defaultSaberModelCon.GetField<SetSaberFakeGlowColor[], SaberModelController>("_setSaberFakeGlowColors");
            for (int i = 0; i < setSaberFakeGlowColors.Length; i++)
            {
                setSaberFakeGlowColors[i].SetField("_colorManager", this._colorManager);
            }
            defaultSaberModelCon.Init(parent, saber);
            baseVisualsGO.SetActive(true);
            return baseVisualsGO;
        }

    }
}