using System;
using System.Threading.Tasks;
using SaberFactory.Helpers;
using SaberFactory.Instances;
using SaberFactory.Models;
using UnityEngine;

namespace SaberFactory
{
    public class MenuSaberProvider
    {
        private readonly SaberInstance.Factory _saberInstanceFactory;
        private readonly SaberSet _saberSet;

        internal MenuSaberProvider(SaberInstance.Factory saberInstanceFactory, SaberSet saberSet)
        {
            _saberInstanceFactory = saberInstanceFactory;
            _saberSet = saberSet;
        }

        public event Action<bool> OnSaberVisibilityRequested;

        public async Task<SaberInstance> CreateSaber(Transform parent, SaberType saberType, Color color, bool createTrail)
        {
            await _saberSet.WaitForFinish();

            var saberModel = saberType == SaberType.SaberA ? _saberSet.LeftSaber : _saberSet.RightSaber;

            var saber = _saberInstanceFactory.Create(saberModel);
            saber.SetParent(parent);
            if (createTrail) saber.CreateTrail(true);
            saber.SetColor(color);

            return saber;
        }

        internal void RequestSaberVisiblity(bool visible)
        {
            OnSaberVisibilityRequested?.Invoke(visible);
        }
    }
}