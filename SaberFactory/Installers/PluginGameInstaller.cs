//#define TEST_TRAIL


using ModestTree;
using SaberFactory.Configuration;
using SaberFactory.Game;
using SaberFactory.Helpers;
using SaberFactory.Instances.Trail;
using SaberFactory.Misc;
using SaberFactory.Models;
using SiraUtil.Interfaces;
using SiraUtil.Sabers;
using UnityEngine;
using Zenject;

namespace SaberFactory.Installers
{
    internal class PluginGameInstaller : Installer
    {
        public override void InstallBindings()
        {
            if (Container.TryResolve<PlayerTransforms>() is { } playerTransforms &&
                Container.TryResolve<SaberInstanceList>() is { } saberInstanceList)
            {
                saberInstanceList.PlayerTransforms = playerTransforms;
            }

            var config = Container.Resolve<PluginConfig>();
            if (!config.Enabled || Container.Resolve<SaberSet>().IsEmpty)
            {
                return;
            }

            Container.BindInterfacesAndSelfTo<EventPlayer>().AsTransient();

            //Container.BindInterfacesAndSelfTo<AFHandler>().AsSingle();

            if (!Container.HasBinding<ObstacleSaberSparkleEffectManager>())
            {
                Container.Bind<ObstacleSaberSparkleEffectManager>().FromMethod(ObstanceSaberSparkleEffectManagerGetter).AsSingle();
            }
            
            Container.BindInterfacesAndSelfTo<GameSaberSetup>().AsSingle();
            Container.BindInstance(SaberModelRegistration.Create<SfSaberModelController>(300));

#if DEBUG && TEST_TRAIL
            if (Container.TryResolve<LaunchOptions>()?.FPFC ?? false)
            {
                var testerInitData = new SaberMovementTester.InitData { CreateTestingSaber = true };
                Container.BindInterfacesAndSelfTo<SaberMovementTester>().AsSingle().WithArguments(testerInitData);
            }
#endif
        }

        private ObstacleSaberSparkleEffectManager ObstanceSaberSparkleEffectManagerGetter(InjectContext ctx)
        {
            var playerSpaceConverter = Container.TryResolve<PlayerSpaceConvertor>();
            Assert.IsNotNull(playerSpaceConverter, $"{nameof(playerSpaceConverter)} was null");
            return playerSpaceConverter.GetComponentInChildren<ObstacleSaberSparkleEffectManager>();
        }
    }
}
