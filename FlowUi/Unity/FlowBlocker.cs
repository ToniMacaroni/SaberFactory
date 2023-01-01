using HMUI;
using ModestTree;
using UnityEngine;
using UnityEngine.UI;

namespace FlowUi.Runtime
{
    public class FlowBlocker
    {
        public bool IsBlocked
        {
            get => _isBlocked;
            set
            {
                if (value)
                {
                    Block();
                }
                else
                {
                    Unblock();
                }
            }
        }

        public FlowBlocker(CanvasGroup canvasGroup)
        {
            _canvasGroup = canvasGroup;
        }

        public void Block()
        {
            if (_isBlocked)
            {
                return;
            }

            _canvasGroup.alpha = 0.5f;
            _canvasGroup.interactable = false;

            _isBlocked = true;
        }

        public void Unblock()
        {
            if (!_isBlocked)
            {
                return;
            }

            _canvasGroup.alpha = 1;
            _canvasGroup.interactable = true;

            _isBlocked = false;
        }

        private void CreateBlocker()
        {
            Assert.IsNotNull(_canvasGroup, "CanvasGroup is null");
            
            _blocker = new GameObject("FlowBlocker");
            var rect = _blocker.AddComponent<RectTransform>();
            rect.SetParent(_canvasGroup.transform, false);
            EssentialHelpers.GetOrAddComponent<GraphicRaycaster>(_blocker);
            _blocker.AddComponent<Touchable>();
            _blocker.AddComponent<Button>().onClick.AddListener(() => {Debug.Log("Clicked");});
        }

        private readonly CanvasGroup _canvasGroup;
        private bool _isBlocked;
        private GameObject _blocker;
    }
}