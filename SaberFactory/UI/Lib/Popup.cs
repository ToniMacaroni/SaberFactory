using System.Collections.Generic;
using System.Threading.Tasks;
using SaberFactory.Helpers;
using UnityEngine;

namespace SaberFactory.UI.Lib
{
    internal class Popup : CustomParsable
    {
        private Transform _originalParent;
        private AnimationManager _animationManager;

        protected Transform _cachedTransform;
        private CanvasGroup _canvasGroup;
        private CanvasGroup _parentCanvasGroup;

        protected virtual void Awake()
        {
            _cachedTransform = transform;
            _canvasGroup = GetComponent<CanvasGroup>();
            _animationManager = new AnimationManager(0.3f, InAnimation, OutAnimation);
        }

        private void InAnimation(float t)
        {
            _cachedTransform.localScale = new Vector3(t, t, t);
            _canvasGroup.alpha = t;
        }

        private void OutAnimation(float t)
        {
            _canvasGroup.alpha = 1-t;
            _cachedTransform.localScale = new Vector3(1-t, 1-t, 1-t);
        }

        protected async Task AnimateIn()
        {
            await _animationManager.AnimateIn();
        }

        protected async Task AnimateOut()
        {
            await _animationManager.AnimateOut();
        }

        protected async Task Create(bool animated)
        {
            Parse();

            if (animated)
            {
                await AnimateIn();
            }
            else
            {
                _cachedTransform.localScale = Vector3.one;
                _canvasGroup.alpha = 1;
            }

            FadeParentCanvases();

        }

        protected async Task Hide(bool animated)
        {
            ShowParentCanvases();

            if (animated)
            {
                await AnimateOut();
            }

            Unparse();

            if (_originalParent != null)
            {
                transform.SetParent(_originalParent, false);
            }
        }

        protected void ParentToViewController()
        {
            _originalParent = transform.parent;

            var parent = _originalParent;
            if (parent.GetComponent<CustomViewController>() != null) return;

            while (parent != null)
            {
                var vc = parent.GetComponent<CustomViewController>();
                if (vc != null)
                {
                    break;
                }

                parent = parent.parent;
            }

            transform.SetParent(parent, false);
        }

        protected void FadeParentCanvases()
        {
            var parent = transform.parent;

            while (parent != null)
            {
                var vc = parent.GetComponent<CanvasGroup>();
                if (vc != null)
                {
                    _parentCanvasGroup = vc;
                    vc.alpha = 0f;
                    break;
                }

                parent = parent.parent;
            }
        }

        protected void ShowParentCanvases()
        {
            if (_parentCanvasGroup == null) return;
            _parentCanvasGroup.alpha = 1;
            _parentCanvasGroup = null;
        }

        internal class Factory : ComponentPlaceholderFactory<Popup> {}
    }
}