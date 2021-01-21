using System;
using SaberFactory.Instances.CustomSaber;
using SaberFactory.Models;
using SaberFactory.Models.CustomSaber;
using SiraUtil.Tools;
using Zenject;

namespace SaberFactory.Instances
{
    internal class InstanceFactory : IFactory<BasePieceModel, BasePieceInstance>
    {
        private readonly SiraLog _logger;
        private readonly DiContainer _container;

        public InstanceFactory(SiraLog logger, DiContainer container)
        {
            _logger = logger;
            _container = container;
        }

        public BasePieceInstance Create(BasePieceModel model)
        {
            if (model is CustomSaberModel customSaberModel)
            {
                return _container.Instantiate<CustomSaberInstance>(new []{customSaberModel});
            }

            throw new ArgumentException("Type of parameter not handled", nameof(model));
        }
    }
}