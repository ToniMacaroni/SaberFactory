using SaberFactory.Models;
using SaberFactory.UI.CustomSaber.Views;
using SaberFactory.UI.Lib;
using SiraUtil.Logging;
using SiraUtil.Tools;
using UnityEngine;
using VRUIControls;

namespace SaberFactory.UI.CustomSaber
{
    internal class CustomSaberUiComposition : BaseUiComposition
    {
        private readonly SaberSet _saberSet;

        protected CustomSaberUiComposition(
            SiraLog logger,
            CustomScreen.Factory screenFactory,
            BaseGameUiHandler baseGameUiHandler,
            PhysicsRaycasterWithCache physicsRaycaster,
            BsmlDecorator bsmlDecorator,
            SaberSet saberSet)
            : base(logger, screenFactory, baseGameUiHandler, physicsRaycaster, bsmlDecorator)
        {
            _saberSet = saberSet;
        }

        protected override void SetupUI()
        {
            var mainScreenInitData = new CustomScreen.InitData
            (
                "Main Screen",
                new Vector3(-25, -7, 0),
                Quaternion.identity,
                new Vector2(105, 140),
                true
            );

            var navigationInitData = new CustomScreen.InitData(
                "Navigation Screen",
                new Vector3(-95, 0, 0),
                Quaternion.identity,
                new Vector2(30, 70),
                true
            );

            _mainView = AddScreen(mainScreenInitData).CreateViewController<MainView>();
            _navigationView = AddScreen(navigationInitData).CreateViewController<NavigationView>();
        }

        protected override void DidOpen()
        {
            base.DidOpen();
            _navigationView.OnExit += ClosePressed;
            _navigationView.OnCategoryChanged += _mainView.ChangeCategory;
        }

        protected override void DidClose()
        {
            base.DidClose();
            _navigationView.OnExit -= ClosePressed;
            _navigationView.OnCategoryChanged -= _mainView.ChangeCategory;

            _ = _saberSet.Save();
        }

        protected override void SetupTemplates()
        {
            base.SetupTemplates();
            BsmlDecorator.AddTemplate("NavHeight", "70");
            // if (BsmlDecorator.StyleSheetHandler.GetSelector("btn", out var selector))
            // {
            //     Debug.LogWarning($"Found {selector.Name}");
            //     foreach (var rule in selector.GetRules())
            //     {
            //         Debug.LogWarning($"- {rule.Name} = {rule.Value}");
            //     }
            // }
        }

        #region Views

        private MainView _mainView;
        private NavigationView _navigationView;

        #endregion
    }
}