using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMUI;
using SaberFactory.Editor;
using SaberFactory.Models;
using SaberFactory.UI;
using SaberFactory.UI.CustomSaber;
using SaberFactory.UI.Lib;
using SaberFactory.UI.Lib.BSML;
using SiraUtil;
using UnityEngine;
using UnityEngine.Rendering;
using Zenject;
using Object = UnityEngine.Object;

namespace SaberFactory.Installers
{
    internal class MenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<CustomComponentHandler>().AsSingle();

            Container.Bind<BaseGameUiHandler>().AsSingle();
            Container.Bind<EditorInstanceManager>().AsSingle();

            BindUiStuff();

            BindRemoteSabers();

            Container
                .BindFactory<Type, SubView.InitData, SubView, SubView.Factory>()
                .FromFactory<SubViewFactory>();
            Container
                .BindFactory<Type, CustomViewController.InitData, CustomViewController, CustomViewController.Factory>()
                .FromFactory<ViewControllerFactory>();
            Container
                .BindFactory<CustomScreen.InitData, CustomScreen, CustomScreen.Factory>()
                .FromFactory<ScreenFactory>();

            Container.Bind<SaberFactoryUI>().To<CustomSaberUI>().AsSingle();
            Container.Bind<SaberGrabController>().AsSingle();
            Container.BindInterfacesAndSelfTo<Editor.Editor>().AsSingle();

            Container.BindInterfacesAndSelfTo<SaberFactoryMenuButton>().AsSingle();

            Container.Bind<TrailPreviewer>().AsSingle();
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

        private void BindUiStuff()
        {
            BindUiFactory<Popup, Popup.Factory>();
            BindUiFactory<CustomUiComponent, CustomUiComponent.Factory>();
        }

        private FactoryToChoiceIdBinder<GameObject, Type, T> BindUiFactory<T, TFactory>()
        {
            BindStatement bindStatement = Container.StartBinding();
            BindInfo bindInfo = bindStatement.SpawnBindInfo();
            bindInfo.ContractTypes.Add(typeof(TFactory));
            FactoryBindInfo factoryBindInfo = new FactoryBindInfo(typeof(TFactory));
            bindStatement.SetFinalizer(new PlaceholderFactoryBindingFinalizer<T>(bindInfo, factoryBindInfo));
            return new FactoryToChoiceIdBinder<GameObject, Type, T>(Container, bindInfo, factoryBindInfo);
        }
    }
}