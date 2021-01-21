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
    internal class CustomViewController : ViewController, INotifyPropertyChanged
    {
        public Action<bool, bool, bool> didActivate;

        protected SubViewHandler _subViewHandler;

        protected virtual string _resourceName => string.Join(".", GetType().Namespace, GetType().Name);

        protected SiraLog _logger;
        protected SubView.Factory _viewFactory;

        [Inject]
        private void Construct(SiraLog logger, SubView.Factory viewFactory)
        {
            _logger = logger;
            _viewFactory = viewFactory;

            _subViewHandler = new SubViewHandler();
        }

        protected override async void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            if (firstActivation)
            {
                await UIHelpers.ParseFromResourceAsync(_resourceName, gameObject, this);
            }
            
            didActivate?.Invoke(firstActivation, addedToHierarchy, screenSystemEnabling);
            _subViewHandler.NotifyDidOpen();
        }

        protected override void DidDeactivate(bool removedFromHierarchy, bool screenSystemDisabling)
        {
            _subViewHandler.NotifyDidClose();
        }

        protected T CreateSubView<T>(Transform parent, bool switchToView = false) where T : SubView
        {
            var initData = new SubView.InitData
            {
                Name = typeof(T).Name,
                Parent = parent
            };

            var view = (T) _viewFactory.Create(typeof(T), initData);
            view.SubViewHandler = _subViewHandler;
            if(switchToView) _subViewHandler.SwitchView(view, false);

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