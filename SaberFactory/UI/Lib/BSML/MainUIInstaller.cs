using System;
using UnityEngine;
using Zenject;

namespace SaberFactory.UI.Lib.BSML
{
    public static class MainUIInstaller
    {
        public static void Install(DiContainer container)
        {
            container.BindInterfacesAndSelfTo<CustomComponentHandler>().AsSingle();
            container.Bind<BaseGameUiHandler>().AsSingle();
            container
                .BindFactory<Type, SubView.InitData, SubView, SubView.Factory>()
                .FromFactory<SubViewFactory>();
            container
                .BindFactory<Type, CustomViewController.InitData, CustomViewController, CustomViewController.Factory>()
                .FromFactory<ViewControllerFactory>();
            container
                .BindFactory<CustomScreen.InitData, CustomScreen, CustomScreen.Factory>()
                .FromFactory<ScreenFactory>();
            
            container.BindInterfacesAndSelfTo<BsmlDecorator>().AsSingle();
            BindUiFactory<Popup, Popup.Factory>(container);
            BindUiFactory<CustomUiComponent, CustomUiComponent.Factory>(container);
        }
        
        private static FactoryToChoiceIdBinder<GameObject, Type, T> BindUiFactory<T, TFactory>(DiContainer container)
        {
            var bindStatement = container.StartBinding();
            var bindInfo = bindStatement.SpawnBindInfo();
            bindInfo.ContractTypes.Add(typeof(TFactory));
            var factoryBindInfo = new FactoryBindInfo(typeof(TFactory));
            bindStatement.SetFinalizer(new PlaceholderFactoryBindingFinalizer<T>(bindInfo, factoryBindInfo));
            return new FactoryToChoiceIdBinder<GameObject, Type, T>(container, bindInfo, factoryBindInfo);
        }
    }
}