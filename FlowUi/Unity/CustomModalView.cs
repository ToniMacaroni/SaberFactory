using System;
using HMUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;
using Object = UnityEngine.Object;
using Screen = HMUI.Screen;

namespace FlowUi.Runtime
{
    public class CustomModalView : MonoBehaviour
    {
        [Inject]
        public readonly DiContainer _container;

        [SerializeField]
        public bool _animateParentCanvas = true;

        public GameObject _blockerGO;

        [SerializeField]
        public PanelAnimationSO _dismissPanelAnimation;

        public bool _isShown;
        public Canvas _mainCanvas;
        public CanvasGroup _parentCanvasGroup;

        [SerializeField]
        public PanelAnimationSO _presentPanelAnimations;

        public Transform _previousParent;
        public int _test;
        public bool _viewIsValid;

        public virtual void OnDisable()
        {
            Hide(false);
        }

        public void OnDestroy()
        {
            if (!(bool)(Object)_blockerGO)
            {
                return;
            }

            Destroy(_blockerGO);
        }

        public void SetupView(Transform screenTransform)
        {
            if (_viewIsValid)
            {
                return;
            }

            gameObject.SetActive(true);
            _mainCanvas = EssentialHelpers.GetOrAddComponent<Canvas>(gameObject);
            if (screenTransform != null)
            {
                _parentCanvasGroup = EssentialHelpers.GetOrAddComponent<CanvasGroup>(screenTransform.gameObject);
                foreach (object component in screenTransform.GetComponents<BaseRaycaster>())
                {
                    var type = component.GetType();
                    if (gameObject.GetComponent(type) == null)
                    {
                        _container.InstantiateComponent(type, gameObject);
                    }
                }
            }
            else
            {
                EssentialHelpers.GetOrAddComponent<GraphicRaycaster>(gameObject);
            }

            EssentialHelpers.GetOrAddComponent<CanvasGroup>(gameObject).ignoreParentGroups = true;
            gameObject.transform.SetParent(screenTransform, true);
            gameObject.SetActive(false);
            _viewIsValid = true;
        }

        public void Hide(bool animated, Action finishedCallback = null)
        {
            if (!_isShown)
            {
                return;
            }

            var componentInParent = transform.GetComponentInParent<ViewController>();
            if (componentInParent != null)
            {
                componentInParent.didDeactivateEvent -= HandleParentViewControllerDidDeactivate;
            }

            Destroy(_blockerGO);
            _isShown = false;
            _dismissPanelAnimation.ExecuteAnimation(gameObject, _animateParentCanvas ? _parentCanvasGroup : null, !animated, () =>
            {
                transform.SetParent(_previousParent, true);
                gameObject.SetActive(false);
                var action = finishedCallback;
                if (action == null)
                {
                    return;
                }

                action();
            });
        }

        public void Show(bool animated, bool moveToCenter = false, Action finishedCallback = null)
        {
            if (_isShown)
            {
                return;
            }

            Canvas canvas;
            ViewController viewController;
            var modalRootTransform = ModalView.GetModalRootTransform(this.transform.parent, out canvas, out viewController);
            if (viewController != null)
            {
                viewController.didDeactivateEvent += HandleParentViewControllerDidDeactivate;
            }

            _previousParent = this.transform.parent;
            if (!_viewIsValid)
            {
                SetupView(modalRootTransform);
            }

            gameObject.SetActive(true);
            gameObject.GetComponent<Canvas>().sortingLayerID = canvas.sortingLayerID;
            if (moveToCenter)
            {
                this.transform.SetParent(modalRootTransform, false);
                var transform = (RectTransform)this.transform;
                transform.pivot = new Vector2(0.5f, 0.5f);
                var center = ((RectTransform)modalRootTransform).rect.center;
                transform.localPosition = new Vector3(center.x, center.y, transform.localPosition.z);
            }
            else
            {
                this.transform.SetParent(modalRootTransform, true);
                var rectTransform = (RectTransform)modalRootTransform;
                var transform = (RectTransform)this.transform;
                var localPosition = transform.localPosition;
                var num = (float)(localPosition.y - transform.rect.height * 0.5 + rectTransform.rect.height * 0.5);
                if (num < 0.0)
                {
                    localPosition.y -= num;
                    transform.localPosition = localPosition;
                }
            }

            _blockerGO = CreateBlocker();
            _isShown = true;
            _presentPanelAnimations.ExecuteAnimation(gameObject, _animateParentCanvas ? _parentCanvasGroup : null, !animated, finishedCallback);
        }

        public GameObject CreateBlocker()
        {
            var blocker = new GameObject("Blocker");
            var rectTransform = blocker.AddComponent<RectTransform>();
            blocker.AddComponent<CanvasRenderer>();
            Canvas canvas = null;
            for (var parent = _mainCanvas.transform.parent; parent != null; parent = parent.parent)
            {
                canvas = parent.GetComponent<Canvas>();
                if (canvas != null)
                {
                    break;
                }
            }

            rectTransform.SetParent(_mainCanvas.transform.parent, false);
            rectTransform.SetSiblingIndex(_mainCanvas.transform.GetSiblingIndex());
            rectTransform.anchorMin = Vector3.zero;
            rectTransform.anchorMax = Vector3.one;
            rectTransform.sizeDelta = Vector2.zero;
            if (canvas != null)
            {
                foreach (object component in canvas.GetComponents<BaseRaycaster>())
                {
                    var type = component.GetType();
                    if (blocker.GetComponent(type) == null)
                    {
                        _container.InstantiateComponent(type, blocker);
                    }
                }
            }
            else
            {
                EssentialHelpers.GetOrAddComponent<GraphicRaycaster>(blocker);
            }

            blocker.AddComponent<Touchable>();
            blocker.AddComponent<Button>().onClick.AddListener(HandleBlockerButtonClicked);
            return blocker;
        }

        public void HandleBlockerButtonClicked()
        {
            var blockerClickedEvent = this.blockerClickedEvent;
            if (blockerClickedEvent == null)
            {
                return;
            }

            blockerClickedEvent();
        }

        public void HandleParentViewControllerDidDeactivate(
            bool removedFromHierarchy,
            bool screenSystemDisabling)
        {
            var prevAnimateParentCanvas = _animateParentCanvas;
            _animateParentCanvas = screenSystemDisabling;
            Hide(!screenSystemDisabling, () => _animateParentCanvas = prevAnimateParentCanvas);
        }

        public static Transform GetModalRootTransform(
            Transform transform,
            out Canvas canvas,
            out ViewController viewController)
        {
            var componentInParent = transform.GetComponentInParent<Screen>();
            canvas = componentInParent.GetComponentInChildren<Canvas>();
            viewController = componentInParent.GetComponentInChildren<ViewController>();
            return viewController != null ? viewController.transform : canvas.transform;
        }

        public event Action blockerClickedEvent;
    }
}