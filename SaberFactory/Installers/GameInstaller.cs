#define TEST_TRAIL


using System;
using System.Linq;
using SaberFactory.Configuration;
using SaberFactory.Game;
using SaberFactory.Helpers;
using SaberFactory.Models;
using SiraUtil.Interfaces;
using SiraUtil.Tools;
using UnityEngine;
using Zenject;

namespace SaberFactory.Installers
{
    internal class GameInstaller : Installer
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
            if (Environment.GetCommandLineArgs().Any(x => x.ToLower() == "fpfc"))
            {
                var testerInitData = new SaberMovementTester.InitData { CreateTestingSaber = true };
                Container.BindInterfacesAndSelfTo<SaberMovementTester>().AsSingle().WithArguments(testerInitData);
            }
#endif

        }
    }
}