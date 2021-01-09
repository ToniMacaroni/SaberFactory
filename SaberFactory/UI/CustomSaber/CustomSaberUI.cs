using SaberFactory.UI.CustomSaber.Views;
using SaberFactory.UI.Lib;
using SiraUtil.Tools;
using UnityEngine;
using Zenject;

namespace SaberFactory.UI.CustomSaber
{
    internal class CustomSaberUI : SaberFactoryUI
    {
        #region Views
        private MainView _mainView;
        private NavigationView _navigationView;
        #endregion

        protected CustomSaberUI(SiraLog logger, CustomScreen.Factory screenFactory, BaseGameUiHandler baseGameUiHandler, DiContainer container) : base(logger, screenFactory, baseGameUiHandler, container)
        {
        }

        public override void SetupUI()
        {
            var mainScreenInitData = new CustomScreen.InitData
            (
                "Main Screen",
                Vector3.zero,
                Quaternion.identity, 
                new Vector2(200, 150),
                isCurved: true
            );

            var navigationInitData = new CustomScreen.InitData(
                "Navigation Screen",
                new Vector3(-140, 0, 0),
                Quaternion.Euler(0, 305, 0),
                new Vector2(30, 100),
                isCurved: true
            );

            _mainView = AddScreen(mainScreenInitData).CreateViewController<MainView>();
            _navigationView = AddScreen(navigationInitData).CreateViewController<NavigationView>();
        }

        protected override void DidOpen()
        {
            base.DidOpen();
            _mainView.OnClosePressed += ClosePressed;
            _navigationView.OnCategoryChanged += _mainView.ChangeCategory;
        }

        protected override void DidClose()
        {
            base.DidClose();
            _mainView.OnClosePressed -= ClosePressed;
            _navigationView.OnCategoryChanged -= _mainView.ChangeCategory;
        }
    }
}