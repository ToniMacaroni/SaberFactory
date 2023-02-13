using SaberFactory.Editor;
using SaberFactory.Helpers;
using SaberFactory.UI;
using SaberFactory.UI.CustomSaber;
using SaberFactory.UI.Lib;
using SaberFactory.UI.Lib.BSML;
using Zenject;

namespace SaberFactory.Installers
{
    internal class PluginMenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.Bind<EditorInstanceManager>().AsSingle();

            MainUIInstaller.Install(Container);

            Container.Bind<BaseUiComposition>().To<CustomSaberUiComposition>().AsSingle();
            Container.Bind<SaberGrabController>().AsSingle();
            Container.BindInterfacesAndSelfTo<Editor.Editor>().AsSingle();

            Container.BindInterfacesAndSelfTo<SaberFactoryMenuButton>().AsSingle();

            Container.Bind<TrailPreviewer>().AsSingle();

            Container.Bind<MenuSaberProvider>().AsSingle();

            Container.BindInterfacesAndSelfTo<GizmoAssets>().AsSingle();

#if DEBUG
            //Container.Bind<DebugMenu>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
#endif
        }
    }
}