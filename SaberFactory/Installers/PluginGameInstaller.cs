//#define TEST_TRAIL


using SaberFactory.Configuration;
using SaberFactory.Game;
using SaberFactory.Helpers;
using SaberFactory.Models;
using SiraUtil.Interfaces;
using Zenject;

namespace SaberFactory.Installers
{
    internal class PluginGameInstaller : Installer
    {
        public override void InstallBindings()
        {
            var config = Container.Resolve<PluginConfig>();
            if (!config.Enabled || Container.Resolve<SaberSet>().IsEmpty) return;

            if (Container.HasBinding<GameplayCoreSceneSetupData>())
            {
                var sceneSetupData = Container.Resolve<GameplayCoreSceneSetupData>();
                var beatmapData = sceneSetupData.difficultyBeatmap.beatmapData;
                Container.Bind<BeatmapData>().WithId("beatmapdata").FromInstance(beatmapData);
                var lastNoteTime = beatmapData.GetLastNoteTime();
                Container.Bind<float>().WithId("LastNoteId").FromInstance(lastNoteTime);
                Container.BindInterfacesAndSelfTo<EventPlayer>().AsTransient();
            }

            //Container.BindInterfacesAndSelfTo<AFHandler>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameSaberSetup>().AsSingle();
            Container.Bind<IModelProvider>().To<SFSaberProvider>().AsSingle();


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