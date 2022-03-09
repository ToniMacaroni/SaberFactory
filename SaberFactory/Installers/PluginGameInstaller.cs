//#define TEST_TRAIL


using SaberFactory.Configuration;
using SaberFactory.Game;
using SaberFactory.Helpers;
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
            var config = Container.Resolve<PluginConfig>();
            if (!config.Enabled || Container.Resolve<SaberSet>().IsEmpty)
            {
                return;
            }

            Container.BindInterfacesAndSelfTo<EventPlayer>().AsTransient();

            //Container.BindInterfacesAndSelfTo<AFHandler>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameSaberSetup>().AsSingle();
            Container.BindInstance(SaberModelRegistration.Create<SfSaberModelController>(300)).AsSingle();

#if DEBUG && TEST_TRAIL
            if (Container.TryResolve<LaunchOptions>()?.FPFC ?? false)
            {
                var testerInitData = new SaberMovementTester.InitData { CreateTestingSaber = true };
                Container.BindInterfacesAndSelfTo<SaberMovementTester>().AsSingle().WithArguments(testerInitData);
            }
#endif
        }
    }
}