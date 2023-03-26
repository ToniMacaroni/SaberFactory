using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlowUi.Helpers;
using SaberFactory.Editor;
using SaberFactory.Helpers;
using SaberFactory.Models;
using SaberFactory.UI;
using SaberFactory.UI.Flow;
using SaberFactory.UI.Lib;
using SiraUtil;
using UnityEngine;
using Zenject;

namespace SaberFactory.Installers
{
    internal class PluginMenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.Bind<SaberInstanceManager>().AsSingle();

            //MainUIInstaller.Install(Container);

            //BindRemoteSabers();

            //Container.Bind<BaseUiComposition>().To<CustomSaberUiComposition>().AsSingle();
            Container.Bind<SaberGrabController>().AsSingle();
            //Container.BindInterfacesAndSelfTo<Editor.Editor>().AsSingle();

            //Container.BindInterfacesAndSelfTo<SaberFactoryMenuButton>().AsSingle();

            Container.Bind<TrailPreviewer>().AsSingle();

            Container.Bind<MenuSaberProvider>().AsSingle();

            Container.BindInterfacesAndSelfTo<GizmoAssets>().AsSingle();
            
            Container.BindFromAssetBundle<Prefab.Editor>("Resources.Prefabs.SaberFactoryEditor");
            Container.BindFlowUi<SaberFactoryMainUi>().CreateMenuButton("Saber Factory", "Sabers!");
            Container.Bind<SaberListController>().AsTransient();

#if DEBUG
            //Container.Bind<DebugMenu>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
#endif
        }
    }
}