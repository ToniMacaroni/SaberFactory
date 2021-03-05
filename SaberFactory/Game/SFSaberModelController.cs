using System;
using SaberFactory.Helpers;
using SaberFactory.Instances;
using SaberFactory.Models;
using SiraUtil.Interfaces;
using SiraUtil.Tools;
using UnityEngine;
using Zenject;

namespace SaberFactory.Game
{
    public class SFSaberProvider : IModelProvider
    {
        public Type Type => typeof(SFSaberModelController);
        public int Priority => 300;
    }

    internal class SFSaberModelController : SaberModelController, IColorable
    {
        [Inject] private readonly SiraLog _logger = null;
        [Inject] private readonly SaberSet _saberSet = null;
        [Inject] private readonly SaberInstance.Factory _saberInstanceFactory = null;
        [Inject] private readonly GameSaberSetup _gameSaberSetup = null;
        [InjectOptional] private readonly EventPlayer _eventPlayer = null;

        private SaberInstance _saberInstance;
        private Color? _saberColor;

        public override async void Init(Transform parent, Saber saber)
        {
            await _gameSaberSetup.SetupTask;

            transform.SetParent(parent, false);

            var saberModel = saber.saberType == SaberType.SaberA ? _saberSet.LeftSaber : _saberSet.RightSaber;

            _saberInstance = _saberInstanceFactory.Create(saberModel);
            _saberInstance.SetParent(transform);
            _saberInstance.CreateTrail(_saberTrail);
            SetColor(_saberColor ?? _colorManager.ColorForSaberType(_saberInstance.Model.SaberSlot.ToSaberType()));

            _eventPlayer?.SetPartEventList(_saberInstance.Events, saber.saberType);

            _logger.Info("Instantiated Saber");
        }

        public void SetColor(Color color)
        {
            _saberColor = color;
            _saberInstance.SetColor(color);
        }

        public Color Color => _saberColor.GetValueOrDefault();
    }
}