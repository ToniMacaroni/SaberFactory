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

            BindRemoteSabers();

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

        private void BindRemoteSabers()
        {
            BindSaber(new RemoteLocationPart.InitData
            {
                Name = "SF Default",
                Author = "Toni Macaroni",
                RemoteLocation = "https://github.com/ToniMacaroni/SaberFactoryV2/blob/main/Sabers/SFDefault.saber?raw=true",
                Filename = "SF Default.saber",
                CoverPath = "SaberFactory.Resources.Icons.SFSaber_Icon.png"
            });

            BindSaber(new RemoteLocationPart.InitData
            {
                Name = "SF Default 2018",
                Author = "Toni Macaroni",
                RemoteLocation = "https://github.com/ToniMacaroni/SaberFactoryV2/blob/main/Sabers/SFDefault2018.saber?raw=true",
                Filename = "SF Default 2018.saber",
                CoverPath = "SaberFactory.Resources.Icons.SFSaber_Icon.png"
            });

            BindSaber(new RemoteLocationPart.InitData
            {
                Name = "SF Saber",
                Author = "Toni Macaroni",
                RemoteLocation = "https://github.com/ToniMacaroni/SaberFactoryV2/blob/main/Sabers/SFSaber.saber?raw=true",
                Filename = "SF Saber.saber",
                CoverPath = "SaberFactory.Resources.Icons.SFSaber_Icon.png"
            });
        }

        private void BindSaber(RemoteLocationPart.InitData data)
        {
            Container.Bind<RemoteLocationPart>().To<RemoteLocationPart>().AsCached().WithArguments(data);
        }
    }
}