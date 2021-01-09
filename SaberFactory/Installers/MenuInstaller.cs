using System;
using SaberFactory.Editor;
using SaberFactory.Instances;
using SaberFactory.Tests;
using SaberFactory.UI;
using SaberFactory.UI.CustomSaber;
using SaberFactory.UI.Lib;
using UnityEngine;
using Zenject;

namespace SaberFactory.Installers
{
    internal class MenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<LoadingTester>().AsSingle();

            Container.Bind<BaseGameUiHandler>().AsSingle();
            Container.Bind<EditorInstanceManager>().AsSingle();

            Container
                .BindFactory<Type, SubView.InitData, SubView, SubView.Factory>()
                .FromFactory<SubViewFactory>();
            Container
                .BindFactory<Type, Transform, object, CustomUiComponent, CustomUiComponent.Factory>()
                .FromFactory<CustomUiComponentFactory>();
            Container
                .BindFactory<Type, CustomViewController.InitData, CustomViewController, CustomViewController.Factory>()
                .FromFactory<ViewControllerFactory>();
            Container
                .BindFactory<CustomScreen.InitData, CustomScreen, CustomScreen.Factory>()
                .FromFactory<ScreenFactory>();

            Container.Bind<SaberFactoryUI>().To<CustomSaberUI>().AsSingle();
            Container.BindInterfacesAndSelfTo<Editor.Editor>().AsSingle();

            Container.BindInterfacesAndSelfTo<SaberFactoryMenuButton>().AsSingle();
        }
    }
}