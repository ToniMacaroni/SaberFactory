using System;
using System.Threading.Tasks;
using SaberFactory.Helpers;
using SaberFactory.Instances;
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
        [InjectOptional] private readonly AFHandler _afHandler = null;
        [InjectOptional] private readonly EventPlayer _eventPlayer = null;
        [Inject] private readonly GameSaberSetup _gameSaberSetup = null;
        [Inject] private readonly SiraLog _logger = null;
        [Inject] private readonly SaberInstance.Factory _saberInstanceFactory = null;
        [Inject] private readonly SaberSet _saberSet = null;
        private Color? _saberColor;

        private SaberInstance _saberInstance;

        public Color Color
        {
            get => _saberColor.GetValueOrDefault();
            set
            {
                _saberColor = value;
                _saberInstance.SetColor(value);
            }
        }

        public override async void Init(Transform parent, Saber saber)
        {
            await _gameSaberSetup.SetupTask;

            transform.SetParent(parent, false);

            var saberModel = saber.saberType == SaberType.SaberA ? _saberSet.LeftSaber : _saberSet.RightSaber;

            _saberInstance = _saberInstanceFactory.Create(saberModel);
            _saberInstance.SetParent(transform);
            _saberInstance.CreateTrail(false, _saberTrail);
            Color = _saberColor ?? _colorManager.ColorForSaberType(_saberInstance.Model.SaberSlot.ToSaberType());

            if (_afHandler != null && AFHandler.IsValid && AFHandler.ShouldFire)
            {
                await Task.Delay(4000);
                await _afHandler.Shoot(this, saber.saberType);
            }
            else
            {
                _eventPlayer?.SetPartEventList(_saberInstance.Events, saber.saberType);
            }

            _logger.Info("Instantiated Saber");
        }
    }
}