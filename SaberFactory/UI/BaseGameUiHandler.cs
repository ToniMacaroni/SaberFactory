using IPA.Utilities;
using UnityEngine;
using HMUI;
using Screen = HMUI.Screen;

namespace SaberFactory.UI
{
    internal class BaseGameUiHandler
    {
        private readonly HierarchyManager _hierarchyManager;
        private readonly ScreenSystem _screenSystem;

        private ViewController _left;
        private ViewController _right;
        private ViewController _main;
        private ViewController _bottom;
        private ViewController _top;

        private BaseGameUiHandler(HierarchyManager hierarchyManager)
        {
            _hierarchyManager = hierarchyManager;
            _screenSystem = hierarchyManager.gameObject.GetComponent<ScreenSystem>();
        }

        public void DismissGameUI()
        {
            _left = GetViewController(_screenSystem.leftScreen);
            _right = GetViewController(_screenSystem.rightScreen);
            _main = GetViewController(_screenSystem.mainScreen);
            _bottom = GetViewController(_screenSystem.bottomScreen);
            _top = GetViewController(_screenSystem.topScreen);

            _screenSystem.leftScreen.SetRootViewController(null, ViewController.AnimationType.Out);
            _screenSystem.rightScreen.SetRootViewController(null, ViewController.AnimationType.Out);
            _screenSystem.mainScreen.SetRootViewController(null, ViewController.AnimationType.Out);
            _screenSystem.bottomScreen.SetRootViewController(null, ViewController.AnimationType.Out);
            _screenSystem.topScreen.SetRootViewController(null, ViewController.AnimationType.Out);
        }

        public void PresentGameUI()
        {
            _screenSystem.leftScreen.SetRootViewController(_left, ViewController.AnimationType.In);
            _screenSystem.rightScreen.SetRootViewController(_right, ViewController.AnimationType.In);
            _screenSystem.mainScreen.SetRootViewController(_main, ViewController.AnimationType.In);
            _screenSystem.bottomScreen.SetRootViewController(_bottom, ViewController.AnimationType.In);
            _screenSystem.topScreen.SetRootViewController(_top, ViewController.AnimationType.In);
        }

        public Transform GetUIParent()
        {
            return _hierarchyManager.transform;
        }

        private ViewController GetViewController(Screen screen)
        {
            return screen.GetField<ViewController, Screen>("_rootViewController");
        }
    }
}