using System;
using System.Reflection;
using SaberFactory.Configuration;
using SaberFactory.Game;
using SaberFactory.Helpers;
using SaberFactory.Models;
using SaberFactory.UI.Lib;
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
                var lastNoteTime = sceneSetupData.difficultyBeatmap.beatmapData.GetLastNoteTime();
                Container.Bind<float>().WithId("LastNoteId").FromInstance(lastNoteTime);
                Container.BindInterfacesAndSelfTo<EventPlayer>().AsTransient();
            }

            Container.Bind<IModelProvider>().To<SFSaberProvider>().AsSingle();
        }
    }
}