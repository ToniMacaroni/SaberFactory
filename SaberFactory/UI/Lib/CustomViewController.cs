using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using HMUI;
using SaberFactory.Helpers;
using SiraUtil.Tools;
using UnityEngine;
using Zenject;

namespace SaberFactory.UI.Lib
{
    internal class CustomViewController : ViewController
    {
        public Action<bool, bool, bool> didActivate;

        protected SubViewHandler _subViewHandler;

        protected virtual string _resourceName => string.Join(".", GetType().Namespace, GetType().Name);

        protected SiraLog _logger;
        protected CustomUiComponent.Factory _componentFactory;
        protected SubView.Factory _viewFactory;

        [Inject]
        private void Construct(SiraLog logger, CustomUiComponent.Factory componentFactory, SubView.Factory viewFactory)
        {
            _logger = logger;
            _componentFactory = componentFactory;
            _viewFactory = viewFactory;

            _subViewHandler = new SubViewHandler();
        }

        protected override async void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            if (firstActivation)
            {
                await UIHelpers.ParseFromResource(_resourceName, gameObject, this);
            }
            
            didActivate?.Invoke(firstActivation, addedToHierarchy, screenSystemEnabling);
        }

        protected T CreateComponent<T>(Transform parent, object componentParams) where T : CustomUiComponent
        {
            return (T) _componentFactory.Create(typeof(T), parent, componentParams);
        }

        protected T CreateSubView<T>(Transform parent, bool switchToView = false) where T : SubView
        {
            var initData = new SubView.InitData
            {
                Name = typeof(T).Name,
                Parent = parent
            };

            var view = (T) _viewFactory.Create(typeof(T), initData);
            if(switchToView) _subViewHandler.SwitchView(view);

            return view;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            try
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            catch (Exception)
            {
                // ignored
            }
        }

        internal class Factory : PlaceholderFactory<Type, InitData, CustomViewController>{}

        internal struct InitData
        {
            public Transform Parent;
            public CustomScreen Screen;
        }
    }
}