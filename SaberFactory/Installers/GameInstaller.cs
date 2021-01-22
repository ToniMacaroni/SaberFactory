using SaberFactory.Configuration;
using SaberFactory.Game;
using SaberFactory.Helpers;
using SiraUtil.Interfaces;
using Zenject;

namespace SaberFactory.Installers
{
    internal class GameInstaller : Installer
    {
        public override void InstallBindings()
        {
            var config = Container.Resolve<PluginConfig>();
            if (!config.Enabled) return;

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