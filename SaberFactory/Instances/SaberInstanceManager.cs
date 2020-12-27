using System;
using SaberFactory.Models;
using SiraUtil.Tools;

namespace SaberFactory.Instances
{
    internal class SaberInstanceManager
    {
        public event Action<SaberInstance> InstanceCreated;

        public SaberInstance CurrentSaber { get; private set; }
        
        private readonly SiraLog _logger;
        private readonly SaberInstance.Factory _saberFactory;


        public SaberInstanceManager(SiraLog logger, SaberInstance.Factory saberFactory)
        {
            _logger = logger;
            _saberFactory = saberFactory;
        }

        public SaberInstance CreateSaber(SaberModel model)
        {
            CurrentSaber = _saberFactory.Create(model);
            InstanceCreated?.Invoke(CurrentSaber);
            return CurrentSaber;
        }
    }
}