using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using HMUI;
using SiraUtil.Logging;
using SiraUtil.Tools;
using UnityEngine;
using Zenject;

namespace SaberFactory.UI.Lib
{
    internal class CustomViewController : ViewController, INotifyPropertyChanged, IAnimatableUi
    {
        protected virtual string _resourceName => string.Join(".", GetType().Namespace, GetType().Name);
        public Action<bool, bool, bool> didActivate;
        protected SiraLog _logger;
        protected SubView.Factory _viewFactory;

        protected SubViewSwitcher SubViewSwitcher;
        private BsmlDecorator _bsmlDecorator;

        public new virtual IAnimatableUi.EAnimationType AnimationType => IAnimatableUi.EAnimationType.Horizontal;

        public event PropertyChangedEventHandler PropertyChanged;

        [Inject]
        private void Construct(SiraLog logger, SubView.Factory viewFactory, BsmlDecorator bsmlDecorator)
        {
            _logger = logger;
            _viewFactory = viewFactory;
            _bsmlDecorator = bsmlDecorator;

            SubViewSwitcher = new SubViewSwitcher();
        }

        public override async void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            if (firstActivation)
            {
                await _bsmlDecorator.ParseFromResourceAsync(_resourceName, gameObject, this);
            }

            didActivate?.Invoke(firstActivation, addedToHierarchy, screenSystemEnabling);
            SubViewSwitcher.NotifyDidOpen();
        }

        public override void DidDeactivate(bool removedFromHierarchy, bool screenSystemDisabling)
        {
            SubViewSwitcher.NotifyDidClose();
        }

        protected T CreateSubView<T>(Transform parent, bool switchToView = false) where T : SubView
        {
            var initData = new SubView.InitData
            {
                Name = typeof(T).Name,
                Parent = parent
            };

            var view = (T)_viewFactory.Create(typeof(T), initData);
            view.SubViewSwitcher = SubViewSwitcher;
            if (switchToView)
            {
                SubViewSwitcher.SwitchView(view, false);
            }

            return view;
        }

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

        internal class Factory : PlaceholderFactory<Type, InitData, CustomViewController>
        { }

        internal struct InitData
        {
            public Transform Parent;
            public CustomScreen Screen;
        }
    }
}