using HMUI;
using IPA.Utilities;
using UnityEngine;
using Screen = HMUI.Screen;

namespace FlowUi
{
    /// <summary>
    ///     Class to dismiss and present the game ui system
    /// </summary>
    public class BaseGameUiHandler
    {
        private readonly List<GameObject> _deactivatedScreens = new();
        private readonly HierarchyManager _hierarchyManager;
        private readonly ScreenSystem _screenSystem;

        public enum ScreenType
        {
            Main,
            Left,
            Right,
            Top,
            Bottom
        }

        private BaseGameUiHandler(HierarchyManager hierarchyManager)
        {
            _hierarchyManager = hierarchyManager;
            _screenSystem = hierarchyManager.gameObject.GetComponent<ScreenSystem>();
        }

        public void DismissGameUI()
        {
            _deactivatedScreens.Clear();
            DeactivateScreen(_screenSystem.leftScreen);
            DeactivateScreen(_screenSystem.mainScreen);
            DeactivateScreen(_screenSystem.rightScreen);
            DeactivateScreen(_screenSystem.bottomScreen);
            DeactivateScreen(_screenSystem.topScreen);

            //var main = GetViewController(_screenSystem.mainScreen);

            //_childViewControllers.Clear();
            //_childViewControllers.Add(GetViewController(_screenSystem.leftScreen));
            //_childViewControllers.Add(GetViewController(_screenSystem.rightScreen));
            //_childViewControllers.Add(main);
            //_childViewControllers.Add(GetViewController(_screenSystem.bottomScreen));
            //_childViewControllers.Add(GetViewController(_screenSystem.topScreen));
            //GetChildViewControllers(main, _childViewControllers);

            //HideViewControllers(_childViewControllers);
        }

        public void PresentGameUI()
        {
            foreach (var screenObj in _deactivatedScreens)
            {
                screenObj.SetActive(true);
            }
        }

        public Transform GetUIParent()
        {
            return _hierarchyManager.transform;
        }

        public Transform GetScreenTransform(ScreenType screenType) => screenType switch
        {
            ScreenType.Main => _screenSystem.mainScreen.transform,
            ScreenType.Left => _screenSystem.leftScreen.transform,
            ScreenType.Right => _screenSystem.rightScreen.transform,
            ScreenType.Top => _screenSystem.topScreen.transform,
            ScreenType.Bottom => _screenSystem.bottomScreen.transform,
            _ => throw new ArgumentOutOfRangeException(nameof(screenType), screenType, null)
        };

        private void DeactivateScreen(Screen screen)
        {
            var go = screen.gameObject;
            if (go.activeSelf)
            {
                _deactivatedScreens.Add(go);
                go.SetActive(false);
            }
        }

        private void HideViewControllers(IEnumerable<ViewController> vcs)
        {
            var cgs = vcs.NonNull().Select(x => x.GetComponent<CanvasGroup>());
            foreach (var cg in cgs)
            {
                cg.gameObject.SetActive(false);
            }
        }

        private void ShowViewControllers(IEnumerable<ViewController> vcs)
        {
            var cgs = vcs.NonNull().Select(x => x.GetComponent<CanvasGroup>());
            foreach (var cg in cgs)
            {
                cg.gameObject.SetActive(true);
            }
        }

        private void GetChildViewControllers(ViewController vc, List<ViewController> list)
        {
            if (vc.childViewController != null)
            {
                list.Add(vc.childViewController);
                GetChildViewControllers(vc.childViewController, list);
            }
        }

        private ViewController GetViewController(Screen screen)
        {
            return screen.GetField<ViewController, Screen>("_rootViewController");
        }
    }
}