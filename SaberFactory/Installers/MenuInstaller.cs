using System;
using System.Reflection;
using SaberFactory.Editor;
using SaberFactory.UI;
using SaberFactory.UI.CustomSaber;
using SaberFactory.UI.Lib;
using SaberFactory.UI.Lib.BSML;
using UnityEngine;
using Zenject;

namespace SaberFactory.Installers
{
    internal class MenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.Bind<BaseGameUiHandler>().AsSingle();
            Container.Bind<EditorInstanceManager>().AsSingle();

            BindUiStuff();

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
            Container.BindInterfacesAndSelfTo<Editor.Editor>().AsSingle();

            Container.BindInterfacesAndSelfTo<SaberFactoryMenuButton>().AsSingle();

            Container.Bind<TrailPreviewer>().AsSingle();

            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                var attr = Attribute.GetCustomAttribute(type, typeof(UIFactoryAttribute)) as UIFactoryAttribute;
                if (attr == null) continue;
                Console.WriteLine($"Found UI Factory {type.Name} {type.Namespace}");
            }
        }

        private void BindUiStuff()
        {
            Container.BindInterfacesAndSelfTo<CustomComponentHandler>().AsSingle();

            BindUiFactory<Popup, Popup.Factory>();
        }

        private FactoryToChoiceIdBinder<GameObject, Type, T1> BindUiFactory<T1, TFactory>()
        {
            BindStatement bindStatement = Container.StartBinding();
            BindInfo bindInfo = bindStatement.SpawnBindInfo();
            bindInfo.ContractTypes.Add(typeof(TFactory));
            FactoryBindInfo factoryBindInfo = new FactoryBindInfo(typeof(TFactory));
            bindStatement.SetFinalizer((IBindingFinalizer)new PlaceholderFactoryBindingFinalizer<T1>(bindInfo, factoryBindInfo));
            return new FactoryToChoiceIdBinder<GameObject, Type, T1>(Container, bindInfo, factoryBindInfo);
        }
    }
}