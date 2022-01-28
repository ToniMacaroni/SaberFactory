using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HarmonyLib;
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
        }
    }
}