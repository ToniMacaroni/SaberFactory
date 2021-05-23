using System.Threading.Tasks;
using IPA.Utilities;
using SaberFactory.Helpers;
using UnityEngine;
using VRUIControls;
using Zenject;

namespace SaberFactory.UI.Lib
{
    internal class Popup : CustomParsable
    {
        private Transform _originalParent;
        private AnimationManager _animationManager;

        protected Transform _cachedTransform;
        private CanvasGroup _canvasGroup;

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

        }

        protected async Task Hide(bool animated)
        {
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

        internal class Factory : ComponentPlaceholderFactory<Popup> {}
    }
}